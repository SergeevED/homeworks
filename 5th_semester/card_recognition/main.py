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
cv2.imwrite(path_to_images  + '/20_thresh.jpg', thresh)

blur_thresh = thresh

_, contours, _ = cv2.findContours(blur_thresh,cv2.RETR_TREE,cv2.CHAIN_APPROX_NONE)

cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
cv2.drawContours(cont_im, contours, -1, (0,0,0), 3)
cv2.imwrite(path_to_images  + '/40_contours.jpg', cont_im)

big_contours = []
# delete small contours
for cnt in contours:
    #if (len(cnt) > 100) and (abs(cnt[0][0][0] - cnt[-1][0][0]) < 2) and (abs(cnt[0][0][1] - cnt[-1][0][1]) < 2):
    if (len(cnt) > 800):
        big_contours.append(cnt)
#big_contours = sorted(big_contours, key=cv2.contourArea, reverse=True)[:3]

# show stats for biggest contours
orig_image_area = len(origin_image) * len(origin_image[0])
for cnt in big_contours:
    print("{:.4f}".format(100.0 * cv2.contourArea(cnt) / orig_image_area))

cont_im = np.ones((len(origin_image), len(origin_image[0]), 3)) * 255
for cnt in big_contours:
    cv2.drawContours(cont_im, [cnt], -1, (random.randint(40, 255),random.randint(0, 255),random.randint(0, 255)), 3)
cv2.imwrite(path_to_images  + '/50_big_contours.jpg', cont_im)

""""
# find quadrangles - DO NOT WORK
for cnt in contours:
    cnt = cv2.approxPolyDP(cnt, 8, True)
    print("len: " + str(len(cnt)))
    if len(cnt) < 4:
        print("<4")
        continue

    if len(cnt) == 4:
        print("==4")
        cv2.drawContours(cont_im, [cnt], 0, (0, 255, 255), 4)

    # moment
    M = cv2.moments(cnt)
    cx = int(M['m10'] / M['m00'])
    cy = int(M['m01'] / M['m00'])

    distances = [(cx - cnt[i][0][0])**2 + (cy - cnt[i][0][1])**2 for i in range(len(cnt))]
    print("dlen: " + str(len(cnt)))

    cornerNumber = 0
    for i in range(len(distances)):
        if distances[i] >= distances[(i-1) % len(distances)] and distances[i] >= distances[(i+1) % len(distances)]:
            print("corner index:" + str(i))
            cornerNumber += 1

    print(cornerNumber)
    if cornerNumber == 4:
        cv2.drawContours(cont_im, [cnt], 0, (0, 255, 255), 2)

show_image('contours', cont_im)
"""