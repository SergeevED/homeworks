import cv2
import numpy as np
import copy


sample_width = 400
sample_height = 560


def preprocess(image, debug=False):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    _, thresh = cv2.threshold(gray, np.average(gray) * 0.8, 255, cv2.THRESH_BINARY)

    # if upper-right corner is darker than upper-left corner rotate image on 90 degrees
    corner_width = 200
    corner_height = 112
    upper_left_corner = thresh[0:corner_height, 0:corner_width]
    upper_right_corner = thresh[0:corner_height, \
                         sample_width - corner_width:sample_width]
    if (np.average(upper_left_corner) > np.average(upper_right_corner)):
        h = np.array([[0, sample_height], [sample_width, sample_height], [sample_width, 0], [0, 0]], np.float32)
        contour = np.array([(0, 0), (0, sample_height), (sample_width, sample_height), (sample_width, 0)], np.float32)
        transform = cv2.getPerspectiveTransform(contour, h)
        thresh = cv2.warpPerspective(thresh, transform, (sample_width, sample_height))

    thresh_not_corrupted = copy.deepcopy(thresh)

    contours, _ = cv2.findContours(thresh, cv2.RETR_TREE, cv2.CHAIN_APPROX_NONE)

    origin_image_area = len(image) * len(image[0])

    filtered_contours = []
    for cnt in contours:
        cnt_area = cv2.contourArea(cnt)
        if 0.5 <= 100.0 * cnt_area / origin_image_area <= 95:
            filtered_contours.append(cnt)

    merged_contours = []
    for cnt in filtered_contours:
        for point in cnt:
            merged_contours.append(point)

    cont_im = np.ones((len(image), len(image[0]), 3)) * 255
    cv2.drawContours(cont_im, merged_contours, -1, (0, 0, 0), 3)

    x, y, w, h = cv2.boundingRect(np.asarray(merged_contours))
    cv2.rectangle(cont_im, (x, y), (x + w, y + h), (0, 255, 0), 2)

    cut_im = thresh_not_corrupted[y:y + h, x:x + w]
    cut_im = cv2.resize(cut_im, (sample_width, sample_height))

    if debug:
        cv2.imwrite("debug_proc_im/00_origin.jpg", image)
        cv2.imwrite("debug_proc_im/10_gray.jpg", gray)
        cv2.imwrite("debug_proc_im/20_thresh.jpg", thresh_not_corrupted)
        cv2.imwrite("debug_proc_im/25_upper_right.jpg", upper_right_corner)
        cv2.imwrite("debug_proc_im/30_upper_left.jpg", upper_left_corner)
        cv2.imwrite("debug_proc_im/40_cut_im.jpg", cut_im)

    return cut_im


#image = cv2.imread("data_gen/cards/Qd.jpg")
#image = cv2.imread("out/card_01.jpg")
#preprocess(image, True)
