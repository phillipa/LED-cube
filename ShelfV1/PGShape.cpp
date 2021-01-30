#include "PGShape.h"

#include <FastLED.h>

//empty default constructor 
PGShape::PGShape()
{
  
}

void PGShape::initShape(CRGB p_color, uint16_t p_sides[], uint16_t p_num_sides, byte side_len[])
{
  color = p_color; 
  num_sides = p_num_sides;
  for(int i = 0; i < p_num_sides; i++)  
    sides[i].initSide(p_sides[i], side_len, color);
}

void PGShape::setColor(CRGB p_color)
{
  color = p_color;
  for(int s = 0; s < num_sides; s++)
    sides[s].setColor(p_color);
}

void PGShape::drawShape(CRGB* leds)
{
  for(int s = 0; s < num_sides; s++)
    sides[s].drawSide(leds);
}



