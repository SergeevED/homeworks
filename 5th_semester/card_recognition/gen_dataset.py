from __future__ import division

import os
import cv2

input_path = "data_gen/cards/"
output_path = "data/"

for input_filename in os.listdir(input_path):
    card_name, card_ext = os.path.splitext(input_filename)
    image = cv2.imread(input_path + input_filename)

    for counter in range(1, 1):
        # TODO: apply effects
        edited_image = image

        # write edited image
        cv2.imwrite(output_path + card_name + "_" + str(counter) + card_ext, edited_image)
