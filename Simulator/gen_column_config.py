#!/usr/bin/env python3
import math 
import yaml
# Generate a config file of addresses to LEDs for the 
# column. Parameters are for size of the column and strip. 

# 18 inches is ~45cm 
column_r = 45/2

# LEDs in circumference and height
circ_leds = 51
height_leds = 72

# 51*72 = 3672  (from #defines in the code)
# 480*8 = 3840  (from other #defines in the code)
# difference is 168, which isn't a whole strip. WTF. 

theta_per_led = math.radians(360/circ_leds)
rise_per_led = 2

theta = 0
turn_count = 0
config = {}
for idx in range(circ_leds*height_leds):
    x = math.sin(theta) * column_r
    y = math.cos(theta) * column_r
    z = (theta/(2*math.pi) * rise_per_led) + (turn_count * rise_per_led)
    theta += theta_per_led
    if theta > (2*math.pi):
        theta = theta %(2* math.pi)
        turn_count += 1
    config[idx] = (x, y, z)

with open("column.yml", 'w') as outfile:
    yaml.dump(config, outfile)

    