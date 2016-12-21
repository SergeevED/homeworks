import numpy as np
import cv2


def show_image(image_name, image):
    cv2.imshow(image_name, image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()


filename = "cards.jpg"

origin_image = cv2.imread(filename)
show_image('origin_image', origin_image)

gray = cv2.cvtColor(origin_image, cv2.COLOR_BGR2GRAY)
show_image('gray', gray)

blur = cv2.GaussianBlur(gray,(1,1),1000)
show_image('blur', blur)

flag, thresh = cv2.threshold(blur, 120, 255, cv2.THRESH_BINARY)
show_image('thresh', thresh)

contours, hierarchy = cv2.findContours(thresh,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)

cont_im = np.ones((800, 856, 3))
cv2.drawContours(cont_im, contours, -1, (0,0,0), 3)
show_image('contours', cont_im)
