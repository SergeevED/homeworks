import numpy as np
import cv2
import random
import os
import copy
import sys

from operator import itemgetter

from util import preprocess


def show_image(image_name, image_to_show):
    cv2.imshow(image_name, image_to_show)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


output_path = "out/"
if not os.path.exists(output_path):
    os.makedirs(output_path)

samples_path = "classifier_samples/"
sample_width = 400
sample_height = 560


def preprocess_orig(orig_image):
    gray = cv2.cvtColor(orig_image, cv2.COLOR_BGR2GRAY)
    if __debug__:
        cv2.imwrite(os.path.join(output_path, '005_gray.jpg'), gray)
    blur = gray
    thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
    if __debug__:
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
    return filtered_contours


# rotate and scale cards
def get_cards_orthographic(origin_image, filtered_contours):
    cards = []
    for i in range(len(filtered_contours)):
        flag = False
        for j in range(i):
            for k in range(4):
                if cv2.pointPolygonTest(filtered_contours[j], tuple(filtered_contours[i][k][0]), False) > 0 or \
                                cv2.pointPolygonTest(filtered_contours[i], tuple(filtered_contours[j][k][0]),
                                                     False) > 0:
                    flag = True
                    break
            if flag:
                break

        if flag:
            continue

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
        cv2.imwrite(os.path.join(output_path, "card_{:02}.jpg".format(i)), warp)
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
                out_samples.append((sample_card_name, sample_card_image))
    return out_samples


def img_diff(img1, img2):
    diff = cv2.absdiff(img1, img2)
    diff = cv2.GaussianBlur(diff, (5, 5), 5)
    _, diff = cv2.threshold(diff, 200, 255, cv2.THRESH_BINARY)
    return np.sum(diff)


def find_closest_card(raw_img, samples, verbose=False):
    assert (len(raw_img) == sample_height)
    assert (len(raw_img[0]) == sample_width)
    img = preprocess(raw_img)
    img = cv2.equalizeHist(img)
    diff_results = [(sample[0], img_diff(img, sample[1])) for sample in samples]
    sorted_diff_results = sorted(diff_results, key=itemgetter(1))
    if verbose:
        print("Diff for another image:")
        for (card_name, diff) in sorted_diff_results[:20]:
            print("\t\t{}\t{}".format(card_name, diff))
    return sorted_diff_results[0][0]


# classify found cards
def classify_cards(cards, samples, verbose=False):
    for card in cards:
        print(find_closest_card(card, samples, verbose=verbose))


def extract_cards_orthographic(filename):
    # load original image
    origin_image = cv2.imread(filename)

    # preprocess
    image = preprocess_orig(origin_image)

    # find contours
    contours, _ = cv2.findContours(image, cv2.RETR_TREE, cv2.CHAIN_APPROX_NONE)
    if __debug__:
        cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
        cv2.drawContours(cont_im, contours, -1, (0, 0, 0), 3)
        cv2.imwrite(os.path.join(output_path, '040_contours.jpg'), cont_im)

    # filter contours
    origin_image_area = len(origin_image) * len(origin_image[0])
    filtered_contours = filter_contours(contours, origin_image_area)
    if __debug__:
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


def main():
    filename = sys.argv[1] if len(sys.argv) > 1 else "images/t3.jpg"

    # cards = extract_cards_orthographic(filename)
    cards = load_cards(output_path)

    # classify cards
    samples = load_samples()
    classify_cards(cards, samples, verbose=False)


main()
