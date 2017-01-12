import numpy as np
import cv2
import random
import os
import copy
import sys

from operator import itemgetter

import shutil

from util import preprocess

debug = False

output_path = "out/"
if not os.path.exists(output_path):
    os.makedirs(output_path)

samples_path = "classifier_samples/"
sample_width = 400
sample_height = 560


def show_image(image_name, image_to_show):
    cv2.imshow(image_name, image_to_show)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


def preprocess_orig(orig_image):
    gray = cv2.cvtColor(orig_image, cv2.COLOR_BGR2GRAY)
    if debug:
        cv2.imwrite(os.path.join(output_path, '005_gray.jpg'), gray)
    blur = gray
    thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
    if debug:
        cv2.imwrite(os.path.join(output_path, '020_thresh.jpg'), thresh)
    blur_thresh = thresh
    return blur_thresh


def filter_contours(contours, origin_image_area):
    filtered_contours = []
    # filter too small or too large contours
    for cnt in contours:
        cnt_area = cv2.contourArea(cnt)
        if 0.1 <= 100.0 * cnt_area / origin_image_area <= 95:
            epsilon = 0.075 * cv2.arcLength(cnt, True)
            approx = cv2.approxPolyDP(cnt, epsilon, True)
            if len(approx) == 4:
                filtered_contours.append(approx)

    contours_to_delete = []
    for i in range(len(filtered_contours)):
        flag = False
        for j in range(i):
            for k in range(4):
                if cv2.pointPolygonTest(filtered_contours[j], tuple(filtered_contours[i][k][0]), False) > 0 or \
                                cv2.pointPolygonTest(filtered_contours[i], tuple(filtered_contours[j][k][0]),
                                                     False) > 0:
                    flag = True
                    contours_to_delete.append(i)
                    break
            if flag:
                break
    filtered_contours = [filtered_contours[i] for i in range(len(filtered_contours)) if i not in contours_to_delete]

    def by_center_location_key(contour):
        # top to bottom, left to right
        moments = cv2.moments(contour)
        center_x = int(moments['m10'] / moments['m00'])
        center_y = int(moments['m01'] / moments['m00'])
        return center_y, center_x

    sorted_contours = sorted(filtered_contours, key=by_center_location_key)
    return sorted_contours


# rotate and scale cards
def get_cards_orthographic(origin_image, filtered_contours):
    cards = []
    for i in range(len(filtered_contours)):

        # if first and second vertices are a small side take vertices starting from next vertex
        first_side = (filtered_contours[i][0][0][0] - filtered_contours[i][1][0][0]) ** 2 + \
                     (filtered_contours[i][0][0][1] - filtered_contours[i][1][0][1]) ** 2
        second_side = (filtered_contours[i][1][0][0] - filtered_contours[i][2][0][0]) ** 2 + \
                      (filtered_contours[i][1][0][1] - filtered_contours[i][2][0][1]) ** 2
        if first_side > second_side:
            temp = copy.deepcopy(filtered_contours[i][0])
            filtered_contours[i][0] = filtered_contours[i][1]
            filtered_contours[i][1] = filtered_contours[i][2]
            filtered_contours[i][2] = filtered_contours[i][3]
            filtered_contours[i][3] = temp

        if cv2.contourArea(filtered_contours[i], True) < 0:
            filtered_contours[i] = filtered_contours[i][::-1]

        contour = np.array([[float(arr[0][0]), float(arr[0][1])] for arr in filtered_contours[i]], np.float32)
        h = np.array([[0, 0], [sample_width, 0], [sample_width, sample_height], [0, sample_height]], np.float32)
        transform = cv2.getPerspectiveTransform(contour, h)
        warp = cv2.warpPerspective(origin_image, transform, (sample_width, sample_height))
        cards.append(warp)
        cv2.imwrite(os.path.join(output_path, "card_{:02}.jpg".format(i + 1)), warp)
    return cards


def load_samples():
    suits = ['d', 'h', 'c', 's']
    ranks = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A']
    out_samples = []
    for rank in ranks:
        for suit in suits:
            sample_card_name = rank + suit
            filepath = os.path.join(samples_path, sample_card_name + '.jpg')
            # read grayscale image
            sample_card_image = cv2.imread(filepath, 0)
            if sample_card_image is not None:
                out_samples.append((sample_card_name, sample_card_image, rotate_180(sample_card_image)))
    return out_samples


def img_diff(img1, img2, verbose=False):
    diff = cv2.absdiff(img1, img2)
    diff = cv2.GaussianBlur(diff, (5, 5), 5)
    _, diff = cv2.threshold(diff, 200, 255, cv2.THRESH_BINARY)

    # debug code
    if verbose:
        cv2.imwrite("out/img1.jpg", img1)
        cv2.imwrite("out/img2.jpg", img2)
        print("diff = {}".format(np.sum(diff)))

    return np.sum(diff)


def rotate_180(img):
    height = len(img)
    width = len(img[0])
    center = (width / 2, height / 2)
    matrix = cv2.getRotationMatrix2D(center, 180, 1.0)
    return cv2.warpAffine(img, matrix, (width, height))


def card_diff(found_card, sample, rotated_sample, verbose=False):
    diff1 = img_diff(found_card, sample, verbose=verbose)
    diff2 = img_diff(found_card, rotated_sample, verbose=verbose)
    return (diff1 + diff2) / 2


def find_closest_card(raw_img, samples, verbose=False):
    assert (len(raw_img) == sample_height)
    assert (len(raw_img[0]) == sample_width)
    img = preprocess(raw_img)
    # img = cv2.equalizeHist(img)
    diff_results = [(sample[0], card_diff(img, sample[1], sample[2])) for sample in samples]
    sorted_diff_results = sorted(diff_results, key=itemgetter(1))
    if verbose:
        print("Diff for another image:")
        for (card_name, diff) in sorted_diff_results[:5]:
            print("\t\t{}\t{}".format(card_name, diff))
    return sorted_diff_results[0][0]


# classify found cards
def classify_cards(cards, samples, verbose=False):
    return [find_closest_card(card, samples, verbose=verbose) for card in cards]


def extract_cards_orthographic(filename):
    # clear output folder to avoid reading unrelated card images in the future
    shutil.rmtree(output_path)
    os.makedirs(output_path)

    # load original image
    origin_image = cv2.imread(filename)

    # preprocess
    image = preprocess_orig(origin_image)

    # find contours
    contours, _ = cv2.findContours(image, cv2.RETR_TREE, cv2.CHAIN_APPROX_NONE)
    if debug:
        cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
        cv2.drawContours(cont_im, contours, -1, (0, 0, 0), 3)
        cv2.imwrite(os.path.join(output_path, '040_contours.jpg'), cont_im)

    # filter contours
    origin_image_area = len(origin_image) * len(origin_image[0])
    filtered_contours = filter_contours(contours, origin_image_area)
    if debug:
        cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
        for cnt in filtered_contours:
            cv2.drawContours(
                cont_im,
                [cnt],
                -1,
                (random.randint(40, 255), random.randint(0, 255), random.randint(0, 255)),
                3
            )
        cv2.imwrite(os.path.join(output_path, '050_filtered_contours.jpg'), cont_im)

    # extract cards from original image
    cards = get_cards_orthographic(origin_image, filtered_contours)
    return cards


def load_cards(path):
    return [cv2.imread(os.path.join(path, filename)) for filename in os.listdir(path) if filename.startswith("card_")]


def main(use_cached_extracted_cards=False):
    filename = sys.argv[1] if len(sys.argv) > 1 else "images/t1.jpg"

    if not use_cached_extracted_cards:
        cards = extract_cards_orthographic(filename)
        print("Extracted {} cards. Classifying...".format(len(cards)))
    else:
        cards = load_cards(output_path)
        print("Loaded {} cards from {}. Classifying...".format(len(cards), output_path))

    # classify cards
    samples = load_samples()
    answers = classify_cards(cards, samples, verbose=False)
    for card_name in answers:
        print(card_name)


# execute main() when main.py is launched, not imported
if __name__ == "__main__":
    main()
