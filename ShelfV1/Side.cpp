#include "Side.h"

//empty default constructor
Side::Side()
{
  
}

void Side::initSide(byte p_side_num, byte side_len[], CRGB p_color)
{
  side_num=p_side_num;
  color=p_color;

  uint16_t ahead=0;
  for(int i = 0; i < (side_num - 1); i++)
    ahead+= side_len[i];

  start_idx = ahead;
  len = side_len[side_num-1];   
}

void Side::drawSide(CRGB* leds)
{
  for(int i = start_idx; i < start_idx+len;i++)
    leds[i]=color;
}

void Side::setColor(CRGB p_color)
{
  color = p_color; 
}




