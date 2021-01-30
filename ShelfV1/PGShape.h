#ifndef PGSHAPE_H
#define PGSHAPE_H

#include "Side.h"
#include <FastLED.h>

class PGShape
{
  public:
    PGShape();
    void initShape(CRGB p_color, uint16_t p_sides[], uint16_t p_num_sides, byte side_len[]);
    void drawShape(CRGB* leds);
    void setColor(CRGB p_color);
  private:
    Side sides[10]; //shape has max of 10 sides
    CRGB color;
    uint16_t num_sides;
  
};

#endif
