import numpy as np
import cv2
import random
import os
import copy

from operator import itemgetter

from util import preprocess


def show_image(image_name, image):
    cv2.imshow(image_name, image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


path_to_images = "images"
if not os.path.exists(path_to_images):
    os.makedirs(path_to_images)

path_to_samples = "classifier_samples/"
sample_width = 250
sample_height = 350

filename = "t3.jpg"

origin_image = cv2.imread(filename)

gray = cv2.cvtColor(origin_image, cv2.COLOR_BGR2GRAY)
cv2.imwrite(path_to_images + '/05_gray.jpg', gray)

blur = gray

thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
cv2.imwrite(path_to_images + '/20_thresh.jpg', thresh)

blur_thresh = thresh

_, contours, _ = cv2.findContours(blur_thresh, cv2.RETR_TREE, cv2.CHAIN_APPROX_NONE)

cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
cv2.drawContours(cont_im, contours, -1, (0, 0, 0), 3)
cv2.imwrite(path_to_images + '/40_contours.jpg', cont_im)

filtered_contours = []
# filter contours
for cnt in contours:
    image_area = len(origin_image) * len(origin_image[0])
    cnt_area = cv2.contourArea(cnt)
    if 0.1 <= 100.0 * cnt_area / image_area <= 95:
        epsilon = 0.075 * cv2.arcLength(cnt, True)
        approx = cv2.approxPolyDP(cnt, epsilon, True)
        if len(approx) == 4:
            filtered_contours.append(approx)

# rotate and scale cards
cards = []
for i in range(len(filtered_contours)):
    flag = False
    for j in range(i):
        for k in range(4):
            if cv2.pointPolygonTest(filtered_contours[j], tuple(filtered_contours[i][k][0]), False) > 0 or \
                    cv2.pointPolygonTest(filtered_contours[i], tuple(filtered_contours[j][k][0]), False) > 0:
                flag = True
                break
        if flag:
            break

    if flag:
        continue

    # if first and second vertices are a small side take vertices starting from next vertex
    if (filtered_contours[i][0][0][0] - filtered_contours[i][1][0][0]) ** 2 + \
                    (filtered_contours[i][0][0][1] - filtered_contours[i][1][0][1]) ** 2 > \
                            (filtered_contours[i][1][0][0] - filtered_contours[i][2][0][0]) ** 2 + \
                            (filtered_contours[i][1][0][1] - filtered_contours[i][2][0][1]) ** 2:
        temp = copy.deepcopy(filtered_contours[i][0])
        filtered_contours[i][0] = filtered_contours[i][1]
        filtered_contours[i][1] = filtered_contours[i][2]
        filtered_contours[i][2] = filtered_contours[i][3]
        filtered_contours[i][3] = temp

    if cv2.contourArea(filtered_contours[i], True) < 0:
        filtered_contours[i] = filtered_contours[i][::-1]

    scaled_length = 250
    scaled_width = 350
    contour = np.array([[float(arr[0][0]), float(arr[0][1])] for arr in filtered_contours[i]], np.float32)
    h = np.array([[0, 0], [scaled_length, 0], [scaled_length, scaled_width], [0, scaled_width]], np.float32)
    transform = cv2.getPerspectiveTransform(contour, h)
    warp = cv2.warpPerspective(origin_image, transform, (scaled_length, scaled_width))
    cards.append(warp)
    cv2.imwrite(path_to_images + '/' + str(i) + '_card.jpg', warp)

cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
for cnt in filtered_contours:
    cv2.drawContours(cont_im, [cnt], -1, (random.randint(40, 255), random.randint(0, 255), random.randint(0, 255)), 3)
cv2.imwrite(path_to_images + '/50_filtered_contours.jpg', cont_im)


def load_samples():
    suits = ['d', 'h', 'c', 's']
    ranks = ['2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K', 'A']
    out_samples = []
    for rank in ranks:
        for suit in suits:
            card_name = rank + suit
            image = cv2.imread(os.path.join(path_to_samples, filename, '.jpg'))
            out_samples.append((card_name, image))
    return out_samples


samples = load_samples()


def img_diff(img1, img2):
    diff = cv2.absdiff(img1, img2)
    diff = cv2.GaussianBlur(diff, (5, 5), 5)
    flag, diff = cv2.threshold(diff, 200, 255, cv2.THRESH_BINARY)
    return np.sum(diff)


def find_closest_card(raw_img, verbose=False):
    assert (len(raw_img) == sample_height)
    assert (len(raw_img[0]) == sample_width)
    img = preprocess(raw_img)
    diff_results = [(sample[0], img_diff(img, sample[1])) for sample in samples]
    sorted_diff_results = sorted(diff_results, key=itemgetter(1))
    if verbose:
        print("Diff for another image:")
        for (card_name, diff) in sorted_diff_results[:20]:
            print("\t\t{}\t{}".format(card_name, diff))
    return sorted_diff_results[0]
