import os

import cv2
import time

input_path = 'data_gen/cards/'
output_path = 'classifier_samples/'
if not os.path.exists(output_path):
    os.makedirs(output_path)

# suits = ['d', 'h', 'c', 's']
suits = ['h', 's']
# suits = ['s']
# ranks = ['2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K', 'A']
ranks = ['3', '4', 'Q']


# ranks = ['Q']


def preprocess(image):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    # cv2.imwrite(output_path + '/10_gray.jpg', gray)

    denoised = cv2.fastNlMeansDenoising(gray, h=50, templateWindowSize=9, searchWindowSize=21)
    # cv2.imwrite(output_path + '/20_denoised.jpg', denoised)

    # blur = gray
    blur = cv2.GaussianBlur(denoised, (5, 5), 4)
    # cv2.imwrite(output_path + '/30_blur.jpg', blur)

    thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
    # cv2.imwrite(output_path + '/30_thresh.jpg', thresh)

    denoised_thresh = cv2.fastNlMeansDenoising(thresh, h=50, templateWindowSize=9, searchWindowSize=21)
    # cv2.imwrite(output_path + '/40_denoised_thresh.jpg', denoised_thresh)

    # blur_thresh = thresh
    blur_thresh = cv2.GaussianBlur(denoised_thresh, (5, 5), 3)
    return blur_thresh


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
            out_image = preprocess(image)
            cv2.imwrite(output_path + filename, out_image)

            end_one = time.time()
            cnt += 1
            print("{} done ({}/{}), time elapsed: {} sec".format(rank + suit, cnt, total, end_one - start_one))

    total_time = time.time() - start_all
    print("")
    print("Total time elapsed: {} sec".format(total_time))
    print("Average time elapsed: {} sec".format(total_time / totalc))


main()
