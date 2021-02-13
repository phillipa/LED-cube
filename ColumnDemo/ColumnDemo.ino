#define USE_OCTOWS2811
#include <OctoWS2811.h>
#include <FastLED.h>

#define GRID_WIDTH 51
#define GRID_HEIGHT 72

#define NUM_LEDS_PER_STRIP (2*((5*GRID_WIDTH) + 33))
#define NUM_STRIPS 8
#define NUM_LEDS (NUM_STRIPS * NUM_LEDS_PER_STRIP)

CRGB leds[NUM_LEDS];

// Pin layouts on the teensy 3:
// OctoWS2811: 2,14,7,8,6,20,21,5

void setup() {
  LEDS.addLeds<OCTOWS2811>(leds, NUM_LEDS_PER_STRIP);
  LEDS.setBrightness(32);
}

int point_x = 0;
int point_y = 0;

//Move a point
void update_point()
{
  //point_x += 1;
  point_y += 1;

  //point_x = point_x % GRID_WIDTH;
  point_y = point_y % GRID_HEIGHT;
}

int point2addr(int x, int y)
{
  return min(NUM_LEDS, x + (y * GRID_WIDTH));
}

void loop() {

  //Blank the whole strip
  for (int ii = 0; ii < NUM_LEDS; ii++)
  {
    leds[ii] = CRGB::Black;
  }

  // Strip ID, lights the first N leds of each strip
  //  int strip_idx = 1;
  //  for(int ii = 0; ii < NUM_LEDS; ii++)
  //  {
  //    //End of a strip
  //    if((ii % 600) == 0)
  //    {
  //      //Light LEDs for the count
  //      for(int jj = 0; jj < strip_idx; jj++)
  //      {
  //        leds[ii + jj] = CRGB::Blue;
  //      }
  //      strip_idx += 1;
  //    }
  //  }


  leds[point2addr(point_x, point_y)] = CRGB::Red;
  //Set a neighborhood around the point to a color
  leds[point2addr(point_x, (point_y + 1))] = CRGB::Blue;
  leds[point2addr((point_x + 1), point_y)] = CRGB::Green;
  leds[point2addr((point_x + 1), (point_y + 1))] = CRGB::Orange;
  leds[point2addr((point_x + 1), (point_y - 1))] = CRGB::Aqua;
  
  leds[point2addr(point_x, (point_y - 1))] = CRGB::Purple;
  leds[point2addr((point_x - 1), point_y)] = CRGB::Yellow;
  leds[point2addr((point_x - 1), (point_y - 1))] = CRGB::Magenta;
  leds[point2addr((point_x - 1), (point_y + 1))] = CRGB::White;
  

  update_point();

  //Output
  LEDS.show();
  LEDS.delay(100);
}
