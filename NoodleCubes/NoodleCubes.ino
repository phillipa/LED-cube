#define USE_OCTOWS2811
#include <OctoWS2811.h>
#include <FastLED.h>

#include "PGShape.h"
#include "Palettes.h"
#include "Sprite.h"
#include "Twinkle.h"

#define NUM_LEDS_PER_STRIP 273
#define NUM_STRIPS 4
#define NUM_LEDS 4*273
#define BRIGHTNESS 128
CRGB leds[NUM_STRIPS * NUM_LEDS_PER_STRIP];
#define UPDATES_PER_SECOND 15

// Pin layouts on the teensy 3:
// OctoWS2811: 2,14,7,8,6,20,21,5


//variables for drawing cube
byte side_len[] = {91, 91, 91, 91, 91, 91,
                   91, 91, 91, 91, 91, 91
                  };

PGShape cube;

PGShape squares[3];
uint8_t step_size = 25;

//move through the palette separately
uint8_t palette_idx[10];

//sprites
Sprite sprites[10];
uint8_t num_sprites = 10;

//variables for twinkles
Twinkle twinkles[NUM_LEDS];
byte perc_twinkle = 70;

//keep count to switch modes periodically
int counter = 0;
byte sec_between_change = 10;

CRGBPalette16 currentPalette;
TBlendType    currentBlending;


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

void setRandomPalette()
{
  int r = random(0, 160);
  if (r < 20)
    currentPalette = adbasic;
  else if (r < 40)
    currentPalette = purplegreen;
  else if (r < 60)
    currentPalette = darkpurples;
  else if (r < 80)
    currentPalette = bluespurples;
  else if (r < 100)
    currentPalette = blues;
  else if (r < 120)
    currentPalette = greens;
  else if (r < 140)
    currentPalette = yellows;
  else
    currentPalette = whites;
}

void colorSide( int side, CRGB color)
{
  int curr_pixel = 0;
  for (int s = 0; s <= side; s++)
  {
    for (int i = 0; i < side_len[s]; i++)
    {
      if (curr_pixel < NUM_LEDS)
      {
        if (s == (side - 1))
          leds[curr_pixel] = color;
        curr_pixel++;
      }

    }

  }
}

void colorSideTwo( int side, CRGB color1, CRGB color2)
{
  int start_pixel, end_pixel;
  start_pixel = end_pixel = 0;
  
  int curr_pixel = 0;
  for (int s = 0; s <= side; s++)
  {
    for (int i = 0; i < side_len[s]; i++)
    { 
      if (curr_pixel < NUM_LEDS)
      {
        if ((s == (side - 1)) && (i == 0))
        {
          start_pixel = curr_pixel;
          end_pixel = start_pixel + side_len[s];
          break;
        }
        curr_pixel++;
      }
    }
  }
  fill_gradient_RGB(leds, start_pixel, color1, end_pixel, color2);  
}

void setup() {
  LEDS.addLeds<OCTOWS2811>(leds, NUM_LEDS_PER_STRIP);
  LEDS.setBrightness(32);

  colorLEDs(CRGB::Orange);
  FastLED.show();
  FastLED.delay(1000 / 2);


  //set initial palette to whatever
  setRandomPalette();
  currentBlending = LINEARBLEND;

  currentPalette = adbasic;
  //set up per sprite palette indexes
  for (int i = 0; i < num_sprites ; i++)
    palette_idx[i] = random(0, 255);


  //init sprites
  for (int i = 0 ; i < num_sprites; i++)
  {
    sprites[i].initSprite(ColorFromPalette(currentPalette, palette_idx[i]), random(1, 3), random(0, NUM_LEDS), i % 2);
    sprites[i].setDirChangePerc(random(0, 3));
  }

  //init cube for whole cube coloring

  uint16_t sides[12];
  for (int i = 0; i < 12; i++)
    sides[i] = i + 1;
  cube.initShape(ColorFromPalette(currentPalette, palette_idx[0]), sides, 12);

  uint16_t sides0[] = {1, 4, 7, 10};
  uint16_t sides1[] = {2, 5, 8, 11};
  uint16_t sides2[] = {3, 6, 9, 12};
  squares[0].initShape(ColorFromPalette(currentPalette, palette_idx[0]), sides0, 4);
  squares[1].initShape(ColorFromPalette(currentPalette, palette_idx[0] + step_size), sides1, 4);
  squares[2].initShape(ColorFromPalette(currentPalette, palette_idx[0] + 2 * step_size), sides2, 4);


    //init twinkles
    for(int i = 0; i < NUM_LEDS; i++)
      twinkles[i].initTwinkle(perc_twinkle, random(0,255));
      
  //show initialization completed successfully
  colorLEDs(CRGB::Green);
  FastLED.show();
  FastLED.delay(1000 / 2);

  clearLEDs();

}

void loop() {
  counter++;
  if (floor(counter / UPDATES_PER_SECOND) > sec_between_change) //change every few seconds
  {
    setRandomPalette();
    counter = 0;
  }

for(int i = 0; i < NUM_LEDS; i++)
    {
      twinkles[i].updateTwinkle(true);
      twinkles[i].drawTwinkle(leds, i, currentPalette, currentBlending);
    }
/*Coloring squares 
  for (int i = 0; i < 3; i++)
  {
    //update color each time step
    squares[i].setColor(ColorFromPalette(currentPalette, palette_idx[0] + i * step_size, BRIGHTNESS, currentBlending));
    squares[i].drawShape(leds, NUM_LEDS, side_len);
  }

  palette_idx[0]++;
*/

  //blend top into bottom color
  /*
colorSideTwo(2, CRGB::Red, CRGB::Blue);
colorSideTwo(5, CRGB::Red, CRGB::Blue);
colorSideTwo(8, CRGB::Red, CRGB::Blue);
colorSideTwo(11, CRGB::Red, CRGB::Blue);
*/
//Red corner opposite blue corner
/*
colorSide(1, CRGB::Blue);
colorSideTwo(2, CRGB::Blue, CRGB::Red);
colorSide(3, CRGB::Red);
colorSideTwo(4, CRGB::Blue, CRGB:: Red);
colorSide(5, CRGB::Red);
colorSide(6, CRGB::Red);
colorSideTwo(7, CRGB::Red, CRGB::Blue);
colorSideTwo(8, CRGB::Blue, CRGB::Red);
colorSideTwo(9, CRGB::Red, CRGB::Blue);
colorSide(10, CRGB::Blue);
colorSide(11, CRGB::Blue);
colorSideTwo(12, CRGB::Blue, CRGB::Red); */
//columns are colors blend between them 
/*
clearLEDs();
colorSide(2, CRGB::Red);
colorSide(5, CRGB::Yellow);
colorSide(8, CRGB::Green);
colorSide(11, CRGB::Blue);

colorSideTwo(1, CRGB::Blue, CRGB::Red);
colorSideTwo(3, CRGB::Red, CRGB::Yellow);
colorSideTwo(4, CRGB::Red, CRGB::Yellow);
colorSideTwo(10, CRGB::Green, CRGB::Blue);
colorSideTwo(6, CRGB::Yellow, CRGB::Green); 
colorSideTwo(7, CRGB::Yellow, CRGB::Green); 
colorSideTwo(9, CRGB::Green, CRGB::Blue);
colorSideTwo(12, CRGB::Blue, CRGB::Red);
*/

  /* CUBE MODE
    cube.drawShape(leds, NUM_LEDS, side_len);
    palette_idx++;
    cube.setColor(ColorFromPalette(currentPalette, palette_idx, BRIGHTNESS, LINEARBLEND));
  */


  /** Sprite mode
    clearLEDs();
    for(int i = 0; i < num_sprites; i++)
    {
      //update color at each time step
      sprites[i].setColor(ColorFromPalette(currentPalette, palette_idx[i]++, BRIGHTNESS, currentBlending));
      sprites[i].updateSprite();
      sprites[i].drawSprite(leds, NUM_LEDS);
    }

  */

  FastLED.show();
  FastLED.delay(1000 / UPDATES_PER_SECOND);
}
