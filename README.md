# LED-cube

LED-cube code. 

* LEDUDPClient (SharpLights is most up to date version of the C# code) sends data via UDP to the Raspberry Pi to output to the Teensy via USB.

* Raspberry Pi is running:

`netcat -dkl 7777 -u > /dev/ttyACM0 & `

* TeensyUSBReader runs on the controller. It reads from USB, writes to LEDs.

** Notes **

* kill netcat on raspberry pi before unplugging teensy from USB. restart netcat when replugging in the teensy to the pi. (otherwise you get permission denied errors on ttyACM0 

* designed/tested on Teensy 3.1/3.2 with Octo WS2811 adaptor. 
Teensy: https://www.adafruit.com/product/2756?gclid=CjwKCAjw67XpBRBqEiwA5RCocWdtZbowt9jJAlNqRdj7J-Fgfqy6KZvYd3nhyb2visKp8ltbQ4mswRoC64wQAvD_BwE 
Octo: https://www.digikey.com/catalog/en/partgroup/octows2811-adapter-board-for-teensy-3-1/68687

