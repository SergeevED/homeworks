import cv2


def preprocess(image, debug=False):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    denoised = cv2.fastNlMeansDenoising(gray, h=50, templateWindowSize=9, searchWindowSize=21)
    blur = cv2.GaussianBlur(denoised, (5, 5), 2)
    thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
    thresh = (255 - thresh)

    kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (4, 4))
    eroded = cv2.erode(thresh, kernel)
    dilated = cv2.dilate(eroded, kernel)

    dilated = (255 - dilated)

    if debug:
        cv2.imwrite("debug_proc_im/00_origin.jpg", image)
        cv2.imwrite("debug_proc_im/10_denoised.jpg", denoised)
        cv2.imwrite("debug_proc_im/20_blur.jpg", blur)
        cv2.imwrite("debug_proc_im/40_thresh.jpg", thresh)

        cv2.imwrite("debug_proc_im/45_eroded.jpg", eroded)
        cv2.imwrite("debug_proc_im/46_dilated.jpg", dilated)

    return dilated
