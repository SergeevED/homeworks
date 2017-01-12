import cv2
import numpy as np
import os
import time

from util import preprocess

input_path = 'data_gen/cards/'
output_path = 'classifier_samples/'
if not os.path.exists(output_path):
    os.makedirs(output_path)

suits = ['d', 'h', 'c', 's']
ranks = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A']


def cut_corners(image):
    img_width = 500
    img_height = 690
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
