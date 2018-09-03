/* LED Wall
   Reads pixel data from usb as a list of bytes. Three bytes make RGB for a pixel.
   List represents pixels from left ->, top -> bottom
   @author mtownsend
   @since May 2018
*/
#define USE_OCTOWS2811 //PG: don't know why this is here

#include <OctoWS2811.h>
#include<FastLED.h>

const byte DEBUG = 0;
const int CONFIG = WS2811_GRB | WS2811_800kHz;
const int NUM_LEDS_PER_STRIP = 15;
const int NUM_STRIPS = 8; //lies

const int FRAME_SIZE = NUM_LEDS_PER_STRIP * NUM_STRIPS;
const uint8_t DARKEN = 1;
const uint8_t CLR_SIG[] = { 0x01, 0x00, 0x01 }; //send to say this frame is done
const uint8_t RST_SIG[] = { 0x01, 0x01, 0x00 }; //send this to signify new frame start

const int CLR_LEN = 3;
const int RST_LEN = 3;

CRGB leds[FRAME_SIZE]; //the real deal. setting this will set the LEDs
CRGB nextFrame[FRAME_SIZE]; //buffer to hold colors before redraw

//DMAMEM int displayMemory[LEDS_PER_STRIP * NUM_LED_STRIPS];
//int drawingMemory[LEDS_PER_STRIP * NUM_LED_STRIPS];
//OctoWS2811 leds(LEDS_PER_STRIP, displayMemory, drawingMemory, CONFIG);

// Input is indexed from left -> right, top -> bottom
// Actual LEDs are indexed starting in the bottom-left, going up then down
//int convertIndex(int index) {
//  int row = index / COLS;
//  int col = index % COLS;
//  boolean flip = (col / 5) % 2 > 0;
//  if ((col % 2 == 0) != flip) {
//    return ROWS - 1 - row + (col * ROWS);
//  }
//  return row + (col * ROWS);
//}

void displayFrame() {
 
  for (int i = 0; i < FRAME_SIZE; i++) {
    leds[i]=nextFrame[i];
  }
  LEDS.show();
}

void setLEDsColor(CRGB color, boolean test)
{
  for(int i = 0; i < FRAME_SIZE;i++)
  {
   if(!test || i%2 ==0)
    leds[i] = color;
  }
    LEDS.show();
    delay(1000);
}
void setup() {

  LEDS.addLeds<OCTOWS2811>(leds, NUM_LEDS_PER_STRIP);
  LEDS.setBrightness(32);
  
  for (int i = 0; i < FRAME_SIZE; i++) {
    nextFrame[i] = CRGB::HotPink;
  }
 //  leds.begin();
  displayFrame();
  Serial.begin(9600); // USB is always 12 Mbit/sec
}

uint32_t currentColour = 0;
int bytesProcessed = 0;
int pixelIndex = 0;
int clrIndex = 0;
int rstIndex = 0;
void loop()
{
  
  // Read from USB
  uint8_t incomingByte;
  if (Serial.available()) {

    incomingByte = Serial.read();
    //Check for frame end signal
    if (CLR_SIG[clrIndex] == incomingByte) {
      if (++clrIndex >= CLR_LEN) {
        if(DEBUG != 0)
          setLEDsColor(CRGB::Olive,true);
        // Received the clear signal
        pixelIndex = 0;
        bytesProcessed = 0;
        clrIndex = 0;
        currentColour = 0;
        displayFrame();
        return;
      }
    }
    else if (clrIndex > 0) {
      clrIndex = 0;
    }
      //check for frame start signal
      if (RST_SIG[rstIndex] == incomingByte) {
      if (++rstIndex >= RST_LEN) {
                if(DEBUG != 0)
                setLEDsColor(CRGB::Maroon,true);

        // Received the reset signal (throw away any state)
        pixelIndex = 0;
        bytesProcessed = 0;
        rstIndex = 0;
        currentColour = 0;
      //  displayFrame();
        return;
      }
    }
    else if (rstIndex > 0) {
      rstIndex = 0;
    }

    // Pack the byte into the current 24-bit colour int
    currentColour = (currentColour << 8) + (incomingByte * DARKEN);

    if (++bytesProcessed >= 3) {
      // We've processed a full RGB colour, so set the led
      if (pixelIndex < FRAME_SIZE) {
        nextFrame[pixelIndex++] = currentColour;
      }
      currentColour = 0;
      bytesProcessed = 0;
    }
  }
}
