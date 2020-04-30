#/usr/bin/python3 
import time
import socket
import struct
import random
import bitstring
import colorsys

class ArtNetSender(object):
	def __init__(self, host, port):
		#TODO extend with universe and channel config
		self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP
		self.host = host
		self.port = port
		self.sequence = 0 # 0 = No sequence, order doesn't matter
		self.universe_nr = 0

	def send(self, data):
		# Artnet 14 supports 
		# 32,768 universes (16 bit universe)
		# 512 channels per universe
		# LEDs take three channels each, so 170 LEDs/universe
		# or 510 channels actually used per universe
		# A UDP packet can have up to 65,535 bytes, but the MTU for ethernet 
		# is 1500 bytes, so which is still WAAAY more than all the channels 
		# in a universe, so it's safe to pack them as one universe 
		# per packet.
		self.universe_nr = 0
		start_index = self.universe_nr * 510
		while start_index < len(data):
			to_send = data[start_index:min(start_index+510, len(data))]

			#TODO check data values?
			#Create a packet, this was cobbled together from pyartnet
			packet = bytearray()
			packet.extend(map(ord, "Art-Net"))
			packet.append(0x00)          # Null terminate Art-Net
			packet.extend([0x00, 0x50])  # Opcode ArtDMX 0x5000 (Little endian)
			packet.extend([0x00, 0x0e])  # Protocol version 14
			packet.append(self.sequence)              # Sequence,
			packet.append(0x00)                                 # Physical
			packet.append(self.universe_nr & 0xFF)                   # Universe LowByte
			packet.append(self.universe_nr >> 8 & 0xFF)              # Universe HighByte
			packet.extend(struct.pack('>h', len(to_send)))  # Pack the number of channels Big endian
			packet.extend(to_send)
			self.socket.sendto(packet, (host, port))

			#Next universe
			self.universe_nr += 1
			start_index = self.universe_nr * 510
		
class LinearAutomaton(object):
	def __init__(self, length):
		self.len = length
		self.pre_bits = bitstring.BitArray(int=0,length=self.len)
		self.post_bits = bitstring.BitArray(int=0,length=self.len)

		# This is rule 30 in Wolfram's atlas
		self.rule = {0b111: 0b0,
					 0b110: 0b0,
					 0b101: 0b0,
					 0b100: 0b1,
					 0b011: 0b1,
					 0b010: 0b1,
					 0b001: 0b1,
					 0b000: 0b0
					}

	def randomize(self):
		for ii in range(self.len):
			self.pre_bits[ii] = random.randint(0,1)
		#So that the first run of gen() doesn't wipe it out
		self.post_bits = self.pre_bits

	def get_data(self):
		data = []
		for byte in self.post_bits.cut(8):
			data.append(byte.uint)
		return data

	def gen(self):
		self.pre_bits = self.post_bits
		self.post_bits = bitstring.BitArray(int=0, length = self.len)

		# First handle the first and last bits of the array
		first = self.pre_bits[0:2]
		first.append(self.pre_bits[-1:])
		self.post_bits[0] = self.rule[first.uint]

		last = self.pre_bits[-2:]
		last.append(self.pre_bits[0:1])
		self.post_bits[-1] = self.rule[last.uint]

		# And then handle everything in between
		preindex = 0
		postindex = 1
		while postindex < self.len -1:
			predecessor = self.pre_bits[preindex:preindex+3]
			self.post_bits[postindex] = self.rule[predecessor.uint]
			preindex += 1
			postindex += 1

class GlowAgent(object):
	def __init__(self, line_size):
		self.line_size = line_size
		self.position = random.randint(0, line_size-1)
		self.direction = random.choice([-1, 1])
		self.width = random.choice([5,7,9,11])
		# Random high-saturation color
		h,s,l = random.random(), 0.5 + random.random()/2.0, 0.4 + random.random()/5.0
		self.color = [int(256*i) for i in colorsys.hls_to_rgb(h,l,s)]
		self.visualization = [self.color] 
		for ii in range(1, self.width/2):
			self.visualization.insert(0, [self.color[0]>>ii, self.color[1]>>ii, self.color[2]>>ii])
			self.visualization.append([self.color[0]>>ii, self.color[1]>>ii, self.color[2]>>ii])

	def get_pixels(self):
		pixels = [[0,0,0]] * self.line_size
 		pixels[min(0, (self.position-len(self.visualization)/2)): max(self.position-(len(self.visualization)/2), len(self.pixels)-1)] = self.visualization

	def move(self):
		self.position += self.direction
		if self.position > self.line_size-1:
			self.position = self.line_size - 1
			self.direction = -self.direction
		elif self.position < 0:
			self.position = 0
			self.direction = -self.direction	

#IP of the controller board
host = "192.168.2.104"
port = 6454

ga = GlowAgent(90)

ans = ArtNetSender(host, port)
# la = LinearAutomaton(300*8*3)
# la.randomize()

# x= [30,30,30]*300

# # ans.send(x)
# # for ii in range(0, 300, 10):
# # 	x[ii*3] = 255
# # 	ans.send(x)
# # 	time.sleep(1)
# for ii in range (100):
# 	la.gen()
# 	data = (la.get_data())
# 	ans.send(data)
# 	time.sleep(0.1)
