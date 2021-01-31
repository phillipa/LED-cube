#include <FastLED.h>
#include "Palettes.h"
#include "PGShape.h"
#include "Twinkle.h"
#include "Sprite.h"
#include "Side.h"

#define LED_PIN     3
#define NUM_LEDS    288
#define BRIGHTNESS  64
#define LED_TYPE    WS2811
#define COLOR_ORDER GRB

#define UPDATES_PER_SECOND 30 

#define TESTING false 


//visualization modes
//setting these up so they can be combined 
//via | at some point ..
#define TWINKLE 1
#define HEXAGON 2
#define SPRITE 4
#define UNDULATE 8
#define SIDES 16

CRGB leds[NUM_LEDS];

CRGBPalette16 currentPalette;
TBlendType    currentBlending;

//variables for drawing hexagons 
byte side_len[] = {10, 10, 9, 9, 10, 10, 
                  9, 10, 10, 10, 9, 9, 
                  10, 9, 10, 10, 10, 10,
                  9, 10, 10, 9, 10, 9,
                  10, 10, 9, 9, 10, 9};
//array of hexagons
PGShape hexagons[5];
byte num_hexagons = 5;

//array of palette indexes to have each hexagon 
//move through the palette separately
uint8_t palette_idx[5];

//variables for twinkles
Twinkle twinkles[NUM_LEDS];
byte perc_twinkle = 70;
byte perc_twinkle_low = 20;

//sprites mode  
Sprite sprites[5]; 
uint8_t num_sprites = 5;

//sides mode 
Side sides[30];
uint8_t num_sides = 30;

//keep count to switch modes periodically
int counter = 0;
byte mode = UNDULATE;
byte sec_between_change = 60;
int updates_per_second = UPDATES_PER_SECOND;

void setup() {
    delay( 3000 ); // power-up safety delay
    FastLED.addLeds<LED_TYPE, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection( TypicalLEDStrip );
    FastLED.setBrightness(  BRIGHTNESS );

    //indicate it has loaded the code/booted
    colorLEDs(CRGB::DarkOrange);
    FastLED.show();
    FastLED.delay(1000 / 2);

    //set initial palette to whatever
    setRandomPalette();
    currentBlending = LINEARBLEND;

    //set initial mode and speed
    mode = SIDES ;
    updates_per_second = UPDATES_PER_SECOND ; 

    //init hexagons
    uint16_t sides1[] = {1, 2, 3, 28, 29, 30}; 
    palette_idx[0] = random(0,255);
    hexagons[0].initShape(ColorFromPalette( currentPalette, palette_idx[0], BRIGHTNESS, currentBlending),sides1,6, side_len);

    uint16_t sides2[] = {4, 5, 6, 7, 8, 9};
    palette_idx[1] = random(0,255);
    hexagons[1].initShape(ColorFromPalette( currentPalette, palette_idx[1], BRIGHTNESS, currentBlending), sides2, 6, side_len);
    

    uint16_t sides3[] = {10, 11, 24, 25, 26, 27}; 
    palette_idx[2] = random(0,255);
    hexagons[2].initShape(ColorFromPalette( currentPalette, palette_idx[2], BRIGHTNESS, currentBlending), sides3, 6, side_len);
       

    uint16_t sides4[] = {12, 13, 14, 15, 16, 17}; 
    palette_idx[3] = random(0,255);
    hexagons[3].initShape(ColorFromPalette( currentPalette, palette_idx[3], BRIGHTNESS, currentBlending), sides4, 6, side_len);
  

    uint16_t sides5[] = {18, 19, 20, 21, 22, 23}; 
    palette_idx[4] = random(0,255);
    hexagons[4].initShape(ColorFromPalette( currentPalette, palette_idx[4], BRIGHTNESS, currentBlending),sides5,6, side_len);
 

    for(int i = 0 ; i < num_sprites; i++)
    {
      sprites[i].initSprite(ColorFromPalette(currentPalette, palette_idx[i]), random(1,3), random(0,NUM_LEDS), i%2);
      sprites[i].setDirChangePerc(random(0,3)); 
    }
    
    //init twinkles
    for(int i = 0; i < NUM_LEDS; i++)
      twinkles[i].initTwinkle(perc_twinkle_low, random(0,255));

     //init sides 
    for(int i=0; i<num_sides; i++)
      sides[i].initSide(i+1, side_len, palette_idx[i%5]);
      

  //signal that initialization was successful
  colorLEDs(CRGB::Green);
  FastLED.show();
  FastLED.delay(1000 / 2);
  //clear LEDs 
  colorLEDs(CRGB::Black);
  FastLED.show();

}


void testloop()
{

}

void loop()
{
  if(TESTING)
  {
    testloop();
    return;
  }
  counter++;
  if(floor(counter/updates_per_second) > sec_between_change) //change every few seconds
  {
    //reset palette indices 
    for(int i = 0; i < 5 ; i++)
      palette_idx[i] = random(0,255);
      
    setRandomMode();
    setRandomPalette();
    counter = 0;
  }


   if((mode & HEXAGON) > 0)
  {
    for(int i = 0; i < num_hexagons; i++)
    {
      //update color each time step
      hexagons[i].setColor(ColorFromPalette(currentPalette, palette_idx[i]++, BRIGHTNESS, currentBlending));
      hexagons[i].drawShape(leds);
     }
  }
   if((mode & UNDULATE) > 0)
  {
    for(int i = 0; i < NUM_LEDS; i++)
    {
      leds[i] = ColorFromPalette(currentPalette,counter+i, BRIGHTNESS, currentBlending);
    }
  }
   if((mode & SIDES) > 0)
  {
    for(int i = 0; i < num_sides; i++)
    {
      sides[i].setColor(ColorFromPalette(currentPalette,palette_idx[i%5]));
      sides[i].drawSide(leds); 
    }
    for(int i = 0; i < 5; i++)
      palette_idx[i]++;
  }
    if((mode & TWINKLE) > 0)
  {
    for(int i = 0; i < NUM_LEDS; i++)
    {
      twinkles[i].updateTwinkle(true);
      twinkles[i].drawTwinkle(leds, i, currentPalette, currentBlending);
    }
  }
   if ((mode & SPRITE) > 0)
  {
    for(int i = 0; i < num_sprites; i++)
    {
      //update color at each time step
      sprites[i].setColor(ColorFromPalette(currentPalette, palette_idx[i]++, BRIGHTNESS, currentBlending));
      sprites[i].updateSprite(); 
      sprites[i].drawSprite(leds, NUM_LEDS);
    }
  }

  FastLED.show();
  FastLED.delay(1000 / updates_per_second);
  clearLEDs();

}

//color LEDs a solid color
void colorLEDs(CRGB col)
{
  for(int i = 0; i < NUM_LEDS; i++)
    leds[i] = col;
}

//clear LEDs
void clearLEDs()
{
  for(int i = 0; i < NUM_LEDS; i++)
    leds[i] = CRGB::Black;
}


void colorSide( int side, CRGB color)
{
  int curr_pixel = 0;
  for(int s = 0; s <= side; s++)
  { 
    for(int i = 0; i < side_len[s]; i++)
    {
      if(curr_pixel < NUM_LEDS)
      {
        if(s == (side-1))
          leds[curr_pixel] = color;
        curr_pixel++;
      }
   
    }

  }
}

void setRandomMode()
{
  int r = random(0,100);
  if(r < 20)
  {
    mode = UNDULATE;
    updates_per_second = UPDATES_PER_SECOND >> 1; //halfspeed
  }
  else if (r<40)
  {
    mode = TWINKLE;
    updates_per_second = UPDATES_PER_SECOND >> 1 ;//halfspeed
    for(int i = 0; i<NUM_LEDS;i++)
        twinkles[i].setPerc(perc_twinkle);//lower percent of twinkles for compose mode 
  }
  else if (r<60)
  {
    mode = SPRITE;      
    updates_per_second = UPDATES_PER_SECOND ;
  }
  else if(r<80)
  {
    mode = HEXAGON;
    updates_per_second = UPDATES_PER_SECOND ;
  }
  else
  {
    mode = SIDES;
    updates_per_second = UPDATES_PER_SECOND;
  }


}
void setRandomPalette()
{
  int r = random(0,160);
  if(r < 20)
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
  else if(r < 140)
    currentPalette = yellows;
  else 
    currentPalette = whites;
}

