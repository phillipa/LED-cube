#!/usr/bin/env python3

#!/usr/bin/env python3

# Take in a config file, describing a set of cubes
# Generate a set of points that are each LED in the cubes
# and a mapping from 3D points to LED addresses that can be used
# to map colors over the space occupied by the cubes. 
#
# Also teach Ab3nd some linear algebra. Woo rotation matrices. 

# Cube configuration
# A cube has 8 corners. In the interests of having a standard, a cube's
# location is the bottom left front corner, and the side is of unit length, 
# so the "origin cube" is at (0,0,0) and it's corners are (widdershins, bottom to top):
# (0,0,0), (1,0,0), (1,1,0), (0,1,0), (0,0,1), (1,0,1), (1,1,1), (0,1,1)
# Then any other cube can be placed sharing a corner with it, so the config for the 
# 4-cube double-height setup is:
# cube0: (0,0,0), (1,0,0), (1,1,0), (0,1,0), (0,0,1), (1,0,1), (1,1,1), (0,1,1)
# cube1: (0,-1,0), (1,-1,0), (1,0,0), (0,0,0), (0,-1,1), (1,-1,1), (1,0,1), (0,0,1)  
# cube2: (1,0,0), (2,0,0), (2,1,0), (1,1,0), (1,0,1), (2,0,1), (2,1,1), (1,1,1)
# cube3: (0,0,1), (1,0,1), (1,1,1), (0,1,1), (0,0,2), (1,0,2), (1,1,2), (0,1,2)
#                    _
#                3 /_/|_
#                  |_|/_/| 2
# 0 underneath ->  /_/|_|/         ASCII art is elite shit 
#                1 |_|/
#         
# Admittedly, this is _a_ configuration, not _the_ configuration. 

# For converting points to strip addresses, a cube also has a list of octows lines to 
# the points that the strip goes through. For a single cube, this is something like this:
# strip0: (0,0,0), (0,1,0), (0,1,1), (1,1,1) 
# strip1: (0,1,0), (1,1,0), (1,1,1), (1,0,1)
# strip2: (1,1,0), (1,0,0), (1,0,1), (0,0,1)
# strip3: (1,0,0), (0,0,0), (0,0,1), (0,1,1)
# Note that these are 4 corners, so they define 3 lines (a line is between corners)

import yaml
import math
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import axes3d

# Length of a LED tube, in LEDs
led_count = 91

# Given two points, generate count equally spaced points on that line
def gen_coords(point_a, point_b, count):
    # Calculate distance between endpoints and distance between each point
    distance = math.sqrt(sum([pow(a-b, 2) for a,b in zip(point_a, point_b)]))
    dist_pt = distance/count

    # Unit vector from a to b
    v = [b-a for a, b in zip(point_a, point_b)]
    norm = math.sqrt(sum([pow(a, 2) for a in v]))
    u = [a/norm for a in v]

    points = []    
    # Range starts at 1 because there are no LEDs at the corners
    for d in [dist_pt * x for x in range(1, count+1)]:
        pt = [a + (b*c) for a, b, c in zip(point_a, u, [d, d, d])]
        points.append(pt)
    return points

# Cube config in YAML, could be loaded from a config file
config_str="""
cube0: [[0,0,0], [1,0,0], [1,1,0], [0,1,0], [0,0,1], [1,0,1], [1,1,1], [0,1,1]]
strip0: [[0,0,0], [0,1,0], [0,1,1], [1,1,1]]
strip1: [[0,1,0], [1,1,0], [1,1,1], [1,0,1]]
strip2: [[1,1,0], [1,0,0], [1,0,1], [0,0,1]]
strip3: [[1,0,0], [0,0,0], [0,0,1], [0,1,1]]
"""

config = yaml.load(config_str)

addr2point = {}

# Very similar, but for each strip. These points end up in the order that 
# the LEDs are in the strip, so enumerating this list gets LED numbers
strip_points = []
strips = ["strip0", "strip1", "strip2", "strip3"]
#Strip order matters, this is building a list of strip points in the order they occur 
#in the cube
for strip_number, strip_name in enumerate(strips):
    scaled_strip = [[jj * led_count for jj in ii] for ii in config[strip_name]]
    for start, end in zip(scaled_strip, scaled_strip[1:]):
        strip_points.extend(gen_coords( start, end, led_count))

for address, point in enumerate(strip_points):
    addr2point[address] = tuple(point)

with open("cube.yml", 'w') as outfile:
    yaml.dump(addr2point, outfile)