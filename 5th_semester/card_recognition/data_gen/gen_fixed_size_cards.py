from __future__ import division

import cv2
import numpy as np
import os

input_path = "raw_cards/"
output_path = "cards/"
if not os.path.exists(output_path):
    os.makedirs(output_path)

out_height = 690
out_width = 500

# resize images
for filename in os.listdir(input_path):
    image = cv2.imread(input_path + filename)
    resized_image = cv2.resize(image, (out_width, out_height))
    cv2.imwrite(output_path + filename, resized_image)

# print size and dimensions ratio of each output image
tot_ratio = 0
card_count = len(os.listdir(output_path))
for filename in os.listdir(output_path):
    card_name = os.path.splitext(filename)[0]

    img = cv2.imread(output_path + filename)
    height = np.size(img, 0)
    width = np.size(img, 1)
    ratio = height / width
    tot_ratio += ratio
    print("{}\t{}\t{}\t{:.4f}".format(card_name, height, width, ratio))

avg_ratio = tot_ratio / card_count
print("Average ratio: {0:.4f}".format(avg_ratio))
