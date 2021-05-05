#!/usr/bin/env python3

# Column is 8 lines of 600 LEDs = 4800
# Represented as a grid wrapped around a cylinder, it's 75 rows x 52 leds in a row or 
# 3900 LEDs
# That's a 1100 LED difference, one of these is more than a little off
# So just solve the general case!

leds_width = 52 #columns
leds_height = 72 #rows

output = """
typedef struct SpatialLED {{
    int x;
    int y;
    int address;
}} SpatialLED;

#define NUM_LEDS {}
SpatialLED allLEDs[NUM_LEDS] = {{\n
""".format(leds_width * leds_height)

for x in range(leds_width):
    for y in range(leds_height):
        output += "\t{{ .x = {}, .y = {}, .address = {}}},\n".format(x, y, x*y)
output += "};"
