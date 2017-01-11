import numpy as np
import cv2
import random
import os


def show_image(image_name, image):
    cv2.imshow(image_name, image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


path_to_images = "images"
if not os.path.exists(path_to_images ):
    os.makedirs(path_to_images )

filename = "t5.jpg"

origin_image = cv2.imread(filename)

gray = cv2.cvtColor(origin_image, cv2.COLOR_BGR2GRAY)
cv2.imwrite(path_to_images  + '/05_gray.jpg', gray)

blur = gray

thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
cv2.imwrite(path_to_images + '/20_thresh.jpg', thresh)

blur_thresh = thresh


_, contours, _ = cv2.findContours(blur_thresh,cv2.RETR_TREE,cv2.CHAIN_APPROX_NONE)

cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
cv2.drawContours(cont_im, contours, -1, (0,0,0), 3)
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
for i in range(len(filtered_contours)):
    if cv2.contourArea(filtered_contours[i], True) < 0:
        filtered_contours[i] = filtered_contours[i][::-1]
    scaled_length = 250
    scaled_width = 350
    contour = np.array([[float(arr[0][0]), float(arr[0][1])] for arr in filtered_contours[i]], np.float32)
    h = np.array([ [0,0],[scaled_length,0],[scaled_length,scaled_width],[0,scaled_width] ], np.float32)
    transform = cv2.getPerspectiveTransform(contour, h)
    warp = cv2.warpPerspective(origin_image, transform, (scaled_length, scaled_width))
    cv2.imwrite(path_to_images + '/' + str(i) + '_card.jpg', warp)

# show stats for biggest contours
#orig_image_area = len(origin_image) * len(origin_image[0])
#for cnt in filtered_contours:
#    print("{:.4f}".format(100.0 * cv2.contourArea(cnt) / orig_image_area))

cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
for cnt in filtered_contours:
    cv2.drawContours(cont_im, [cnt], -1, (random.randint(40, 255),random.randint(0, 255),random.randint(0, 255)), 3)
cv2.imwrite(path_to_images  + '/50_filtered_contours.jpg', cont_im)
