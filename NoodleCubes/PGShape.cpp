#include "PGShape.h"

#include <FastLED.h>

//empty default constructor 
PGShape::PGShape()
{
  
}

void PGShape::initShape(CRGB p_color, uint16_t* p_sides, uint16_t p_num_sides)
{
  //sides = p_sides;
  for(int i = 0; i < p_num_sides; i++)
    sides[i] = p_sides[i];
    
  color = p_color; 
  num_sides = p_num_sides;
}

void PGShape::setColor(CRGB p_color)
{
  color = p_color;
}

void PGShape::drawShape(CRGB* leds, uint16_t num_leds, byte side_len[])
{
  for(int s = 0; s < num_sides; s++)
    colorSide(leds, num_leds, side_len, sides[s], color);

}

void PGShape::colorSide(CRGB* leds, uint16_t num_leds, byte side_len[], int side, CRGB p_color)
{
  int curr_pixel = 0;
  for(int s = 0; s <= side; s++)
  { 
    for(int i = 0; i < side_len[s]; i++)
    {
      if(curr_pixel < num_leds)
      {
        if(s == (side-1))
          leds[curr_pixel] = p_color;
        curr_pixel++;
      }
   
    }

  }
}
