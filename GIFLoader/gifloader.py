# Proof of concept, load an image and send it over UDP to the column. 
# This treats the column as a 51 column x 75 row display
import cv2
import socket
import time

img = cv2.imread("./checkerboard.jpg", cv2.IMREAD_COLOR)

# Scale image height to 72px (column height)
orig_height, orig_width, _ = img.shape
scale = 72/orig_height
new_width = int(orig_width * scale)
new_height = int(orig_height * scale)

new_img = cv2.resize(img, (new_width, new_height), interpolation=cv2.INTER_CUBIC)
# Crop a 51 px wide section out of the center of it
cropped = new_img[:, int(new_width/2)-25:int(new_width/2)+26]

#Make up your minds, opencv. It should be either imShow or waitkey
# cv2.imshow("resized", cropped)
# cv2.waitKey(0)

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
ip = "192.168.0.228"
port = 21324

# Set up and send UDP packets, first byte sets the protocol
# 1 	WARLS 	255
# 2 	DRGB 	490
# 3 	DRGBW 	367
# 4 	DNRGB 	489/packet
# 0 	WLED Notifier 	-
# We're using DNRGB because we have 75 x 51 = 3825 LEDs

# This is how the bytes are laid out for the color information
# 2 	Start index high byte
# 3 	Start index low byte
# 4 + n*3 	Red Value
# 5 + n*3 	Green Value
# 6 + n*3 	Blue Value

as_ints = [4, 1]
max_yy, max_xx, _ = cropped.shape

# as_ints.extend([1,5, 255,0,0])
# while(True):
#     sock.sendto(bytes(as_ints), (ip, port))
while(True):
    for yy in range(max_yy):
        for xx in range(max_xx):
            # Calculate the LED offset
            offset = (max_xx * xx) + yy
            high_byte, low_byte = offset.to_bytes(2, 'big')
            # Get the RGB values 
            # b, g, r = cropped[yy, xx]
            if yy % 52 == 0:
                b = 0
                g = 0
                r = 200
            else:
                b = 0
                g = 0
                r = 0
            # Limit is actually 489, but I'm playing it safe
            if len(as_ints) > 200:
                #print(as_ints)
                #print("---")
                sock.sendto(bytes(as_ints), (ip, port))
                time.sleep(0.1)
                as_ints = [4,1]
            as_ints.extend([high_byte, low_byte, r, g, b])
    # Last packet out
    sock.sendto(bytes(as_ints), (ip, port))


