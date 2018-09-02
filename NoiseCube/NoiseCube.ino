#define USE_OCTOWS2811
#include<OctoWS2811.h>
#include<FastLED.h>
#include "Palettes.h"

#define NUM_LEDS_PER_STRIP 182
#define NUM_STRIPS 4

#define kMatrixWidth  8
#define kMatrixHeight 91

#define NUM_LEDS (kMatrixWidth * kMatrixHeight)
// Param for different pixel layouts
#define kMatrixSerpentineLayout  true


CRGB leds[NUM_STRIPS * NUM_LEDS_PER_STRIP];

int global_i;
int palette_i;
/***** Noise Variables *****/
// x,y, & time values
uint32_t x,y,v_time,hue_time,hxy;

// Play with the values of the variables below and see what kinds of effects they
// have!  More octaves will make things slower.

// how many octaves to use for the brightness and hue functions
uint8_t octaves=4;
uint8_t hue_octaves=1;

// the 'distance' between points on the x and y axis
int xscale=57771;
int yscale=57771;

// the 'distance' between x/y points for the hue noise
int hue_scale=1;

// how fast we move through time & hue noise
int time_speed=1111;
int hue_speed=1;

// adjust these values to move along the x or y axis between frames
int x_speed=331;
int y_speed=1111;
/******** end noise *****/

/*** start sparks ****/
#define NUM_SPARKS 255
int sparks[NUM_SPARKS];
int perc_sparks=100;

// Pin layouts on the teensy 3:
// OctoWS2811: 2,14,7,8,6,20,21,5

void setup() {
  LEDS.addLeds<OCTOWS2811>(leds, NUM_LEDS_PER_STRIP);
  LEDS.setBrightness(32);

  ///// Noise set up 
 // initialize the x/y and time values
  random16_set_seed(8934);
  random16_add_entropy(analogRead(3));

  hxy = (uint32_t)((uint32_t)random16() << 16) + (uint32_t)random16();
  x = (uint32_t)((uint32_t)random16() << 16) + (uint32_t)random16();
  y = (uint32_t)((uint32_t)random16() << 16) + (uint32_t)random16();
  v_time = (uint32_t)((uint32_t)random16() << 16) + (uint32_t)random16();
  hue_time = (uint32_t)((uint32_t)random16() << 16) + (uint32_t)random16();

  ////////////
  
  global_i=0;
 palette_i=0;
  ////init sparks
  for(int i = 0; i < 255; i++)
  {
      sparks[i] = random(0,255);
  }
}

CRGBPalette16 palettes[] = {adbasic, blues, bluespurples, greens, yellows,darkpurples,HeatColors_p,CloudColors_p, ForestColors_p};
void loop() {

  run_sparks(palettes[palette_i]);
  
  if(global_i > 1000)
  {
    global_i =0;
    palette_i++;
    palette_i %= 9;
  }
}

void run_sparks(CRGBPalette16 palette)
{
  
  for(int i = 0; i < NUM_LEDS; i++)
  { 
    if(sparks[i%NUM_SPARKS]>0) //if the spark corresponding to this LED (i) is lit (>0)
    {
      
      leds[i] = ColorFromPalette(palette, ((int)(global_i/5)+i)%255); //??
      
      for(int d=0; d<(255-sparks[i%NUM_SPARKS]);d++) //some sort of fading. 
      {
        if(leds[i].r >0)
          leds[i].r--;
        if(leds[i].g > 0)
          leds[i].g--;
        if(leds[i].b > 0)
          leds[i].b--;
      }
    }
  }

 //update the spark values.
   for(int i = 0; i < NUM_SPARKS;i++)
   {
      if(sparks[i] > 0)
      {
        sparks[i]--;
          
        if(sparks[i] < 5 || sparks[i] > 255)
          sparks[i] = 0;
         
      }
      else
      {
         if(random(0,100)<perc_sparks)
            sparks[i] = random(0,255);
      }
   }

    global_i++;
    LEDS.show();
    delay(50);
}

void run_noise()
{
  fill_2dnoise16(LEDS.leds(), kMatrixWidth, kMatrixHeight, kMatrixSerpentineLayout,
                octaves,x,xscale,y,yscale,v_time,
                hue_octaves,hxy,hue_scale,hxy,hue_scale,hue_time, false);
                
 
  LEDS.show();

  // adjust the intra-frame time values
  x += x_speed;
  y += y_speed;
  v_time += time_speed;
  hue_time += hue_speed;
  delay(50);
}


void undulate_palette(CRGBPalette16 palette, int sleep_val)
{
  for(int i = 0; i < NUM_STRIPS; i++) {
    for(int j = 0; j < NUM_LEDS_PER_STRIP; j++) {
      leds[(i*NUM_LEDS_PER_STRIP) + j] = ColorFromPalette(palette, (global_i+(i*NUM_LEDS_PER_STRIP)+j)%255);
    }
  }

LEDS.show();
  LEDS.delay(sleep_val);
  }

