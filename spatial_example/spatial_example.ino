#define USE_OCTOWS2811
#include <OctoWS2811.h>
#include <FastLED.h>
#include "lut.h"

//NUM_LEDS defined in lut.h
#define NUM_LEDS_PER_STRIP 273
#define NUM_STRIPS 4
CRGB leds[NUM_LEDS_PER_STRIP * NUM_STRIPS];

//color LEDs a solid color
void colorLEDs(CRGB col)
{
  for (int i = 0; i < NUM_LEDS; i++)
    leds[i] = col;
}

//clear LEDs
void clearLEDs()
{
  for (int i = 0; i < NUM_LEDS; i++)
    leds[i] = CRGB::Black;
}

void setup() {
  LEDS.addLeds<OCTOWS2811>(leds, NUM_LEDS_PER_STRIP);
  LEDS.setBrightness(32);

  //Signal that we entered setup()
  colorLEDs(CRGB::Orange);
  FastLED.show();
  FastLED.delay(1000 / 2);

  //Do any other init here

  //show initialization completed successfully
  colorLEDs(CRGB::Green);
  FastLED.show();
  FastLED.delay(1000 / 2);

}

void loop() {

  // put your main code here, to run repeatedly:
  for (int ii = 0; ii < 91; ii++)
  {
    for (int idx = 0; idx < NUM_LEDS; idx++) {
      if (allLEDs[idx].z < ii) {
        leds[allLEDs[idx].address] = CRGB::Red;
      } else {
        leds[allLEDs[idx].address] = CRGB::Blue;
      }
    }
    FastLED.show();
    delay(100);
  }
  for (int ii = 91; ii > 0; ii--)
  {
    for (int idx = 0; idx < NUM_LEDS; idx++) {
      if (allLEDs[idx].z < ii) {
        leds[allLEDs[idx].address] = CRGB::Red;
      } else {
        leds[allLEDs[idx].address] = CRGB::Blue;
      }
    }
    FastLED.show();
    delay(100);
  }
}
