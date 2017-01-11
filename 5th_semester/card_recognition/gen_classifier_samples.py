import cv2
import numpy as np
import os
import time

input_path = 'data_gen/cards/'
output_path = 'classifier_samples/'
if not os.path.exists(output_path):
    os.makedirs(output_path)

img_width = 500
img_height = 690

suits = ['d', 'h', 'c', 's']
ranks = ['2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K', 'A']


def preprocess(image):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    # cv2.imwrite(output_path + '/10_gray.jpg', gray)

    denoised = cv2.fastNlMeansDenoising(gray, h=50, templateWindowSize=9, searchWindowSize=21)
    # cv2.imwrite(output_path + '/20_denoised.jpg', denoised)

    # blur = gray
    blur = cv2.GaussianBlur(denoised, (5, 5), 2)
    # cv2.imwrite(output_path + '/30_blur.jpg', blur)

    thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
    # cv2.imwrite(output_path + '/30_thresh.jpg', thresh)

    denoised_thresh = cv2.fastNlMeansDenoising(thresh, h=40, templateWindowSize=9, searchWindowSize=21)
    # cv2.imwrite(output_path + '/40_denoised_thresh.jpg', denoised_thresh)

    # blur_thresh = thresh
    blur_thresh = cv2.GaussianBlur(denoised_thresh, (5, 5), 4)
    return blur_thresh


def cut_corners(image):
    x_padding = 0
    y_padding = 0
    corner_width = 110
    corner_height = 340

    left_upper = image[
                 y_padding:(y_padding + corner_height),
                 x_padding:(x_padding + corner_width)
                 ]
    right_lower = image[
                  (img_height - y_padding - corner_height):(img_height - y_padding),
                  (img_width - x_padding - corner_width):(img_width - x_padding)
                  ]
    center = (corner_width / 2, corner_height / 2)
    matrix = cv2.getRotationMatrix2D(center, 180, 1.0)
    right_lower_turned = cv2.warpAffine(right_lower, matrix, (corner_width, corner_height))
    corners_image = np.concatenate((left_upper, right_lower_turned), axis=1)
    return corners_image


def main():
    cnt = 0
    total = len(ranks) * len(suits)
    start_all = time.time()
    for rank in ranks:
        for suit in suits:
            start_one = time.time()
            filename = rank + suit + '.jpg'
            image = cv2.imread(os.path.join(input_path, filename))
            if image is None:
                continue

            # corners_image = cut_corners(image)
            out_image = preprocess(image)
            cv2.imwrite(output_path + filename, out_image)

            end_one = time.time()
            cnt += 1
            print("{} done ({}/{}), time elapsed: {} sec".format(rank + suit, cnt, total, end_one - start_one))

    total_time = time.time() - start_all
    print("")
    print("Total time elapsed: {} sec".format(total_time))
    print("Average time elapsed: {} sec".format(total_time / total))


main()
