import cv2


def preprocess(image):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    denoised = cv2.fastNlMeansDenoising(gray, h=50, templateWindowSize=9, searchWindowSize=21)
    blur = cv2.GaussianBlur(denoised, (5, 5), 2)
    thresh = cv2.adaptiveThreshold(blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY_INV, 9, 0)
    denoised_thresh = cv2.fastNlMeansDenoising(thresh, h=40, templateWindowSize=9, searchWindowSize=21)
    blur_thresh = cv2.GaussianBlur(denoised_thresh, (5, 5), 4)
    return blur_thresh
