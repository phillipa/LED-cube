# LED-cube

LED-cube code. 

* LEDUDPClient sends data via UDP to the Raspberry Pi to output to the Teensy via USB.

* Raspberry Pi is running:

`netcat -dkl 7777 -u > /dev/ttyACM0 & `

* TeensyUSBReader reads from USB, writes to LEDs.
