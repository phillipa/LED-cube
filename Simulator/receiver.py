#!/usr/bin/env python3

# This implements a receiver for the WLED UDP protocol (DNRGB type)
# The idea is that you implement against a simulator that uses
# this receiver and updates some sort of visual display, and then
# for production you use the real LED gear over e.g. wifi.
#
# DNRGB packets are laid out like so:
# Byte      Meaning
# ----      -------
# 2 	    Start index high byte
# 3       	Start index low byte
# 4 + n*3 	Red Value
# 5 + n*3 	Green Value
# 6 + n*3 	Blue Value

import socket
import asyncio
from mpl_toolkits import mplot3d
import os, random
import yaml
import struct
import matplotlib.pyplot as plt
import numpy as np
from matplotlib.animation import FuncAnimation

HOST, PORT = "localhost", 4321


def send_test_message(message):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # Internet  # UDP
    sock.sendto(message, (HOST, PORT))


async def write_messages():
    while True:
        # Make up a starting index
        # TODO this has a hard limit on the max index
        start_idx = random.randint(0, 480)
        end_idx = min(480, start_idx + random.randint(1, 480))
        length = end_idx - start_idx

        # Pack format is unsigned short (2 bytes) followed by R, G, B
        # bytes for each pixel
        pckfmt = "H{}B".format(length * 3)

        # Generate an array of RGB values for each pixel
        data = []
        for ii in range(length):
            data.append(random.randint(0, 255))
            data.append(random.randint(0, 255))
            data.append(random.randint(0, 255))

        msg = struct.pack(pckfmt, start_idx, *data)

        await asyncio.sleep(random.uniform(0.1, 3.0))
        send_test_message(msg)


class DNRGBProtocol(asyncio.DatagramProtocol):
    def __init__(self):
        super().__init__()

    def connection_made(self, transport):
        self.transport = transport

    def datagram_received(self, data, addr):
        data_len = len(data) - 2
        pckfmt = "H{}B".format(data_len)
        start_idx, *rgb_vals = struct.unpack(pckfmt, data)
        print(f"Received message for {(len(data)-2)/3} LEDs")
        print(f"\tStart offset is {start_idx}")
        print(f"\tFirst RGB triplet is {rgb_vals[0]}, {rgb_vals[1]}, {rgb_vals[2]}")


class Visualiser:
    def __init__(self):
        self.fig = plt.figure()
        self.ax = plt.axes(projection="3d")
        self.data = None
        self.colors = None
        

    def plot_init(self, config="./column.yml"):
        self.data = yaml.load(open(config, "r"))
        xdata = []
        ydata = []
        zdata = []
        for idx in self.data.keys():
            x, y, z = self.data[idx]
            xdata.append(x)
            ydata.append(y)
            zdata.append(z)
        self.scatter = self.ax.scatter3D(xdata, ydata, zdata)

    def update_plot(self, frame):
        colors = [np.random.random(3) for _ in self.data.keys()]
        self.scatter._facecolor3d = colors
        pass


if __name__ == "__main__":
    # loop = asyncio.get_event_loop()
    # t = loop.create_datagram_endpoint(DNRGBProtocol, local_addr=('0.0.0.0', PORT))

    vis = Visualiser()
    ani = FuncAnimation(vis.fig, vis.update_plot, init_func=vis.plot_init)

    plt.show(block=True)
    # loop.run_until_complete(t) # Server starts listening
    # loop.run_until_complete(write_messages()) # Start writing messages (or running tests)

    # loop.run_forever()
