import cv2
import numpy as np
import copy

out_width = 400
out_height = 560


def preprocess(image, debug=False):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    _, thresh = cv2.threshold(gray, np.average(gray) * 0.8, 255, cv2.THRESH_BINARY)

    # copy thresh to a bigger canvas to definitely find all contours
    border = 5
    thresh_bigger = np.ones((thresh.shape[0] + border * 2, thresh.shape[1] + border * 2), np.uint8) * 255
    thresh_bigger[border:border + thresh.shape[0], border:border + thresh.shape[1]] = thresh
    thresh = thresh_bigger
    thresh_not_corrupted = copy.deepcopy(thresh)

    contours, _ = cv2.findContours(thresh, cv2.RETR_LIST, cv2.CHAIN_APPROX_NONE)

    origin_image_area = len(thresh) * len(thresh[0])

    filtered_contours = []
    for cnt in contours:
        cnt_area = cv2.contourArea(cnt)
        if 0.5 <= 100.0 * cnt_area / origin_image_area <= 95:
            filtered_contours.append(cnt)

    merged_contours = []
    for cnt in filtered_contours:
        for point in cnt:
            merged_contours.append(point)

    cont_im = np.ones((len(thresh), len(thresh[0])), np.uint8) * 255
    cv2.drawContours(cont_im, merged_contours, -1, (0, 0, 0), 3)

    if len(merged_contours) == 0:
        return cv2.resize(cont_im, (out_width, out_height))

    x, y, w, h = cv2.boundingRect(np.asarray(merged_contours))
    cv2.rectangle(cont_im, (x, y), (x + w, y + h), (0, 255, 0), 2)

    cut_im = thresh_not_corrupted[y:y + h, x:x + w]
    cut_im = cv2.resize(cut_im, (out_width, out_height))

    if debug:
        cv2.imwrite("debug_proc_im/00_origin.jpg", image)
        cv2.imwrite("debug_proc_im/10_gray.jpg", gray)
        cv2.imwrite("debug_proc_im/20_thresh.jpg", thresh_not_corrupted)
        cv2.imwrite("debug_proc_im/30_contours.jpg", cont_im)
        cv2.imwrite("debug_proc_im/40_cut_im.jpg", cut_im)

    return cut_im

# image = cv2.imread("debug_proc_im/00_origin.jpg")
# image = cv2.imread("out/card_11.jpg")
# preprocess(image, True)
