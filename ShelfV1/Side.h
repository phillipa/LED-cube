
#include <FastLED.h>

#ifndef SIDE_H
#define SIDE_H

class Side
{
  public:
    Side();
    void initSide(byte p_side_num, byte side_len[], CRGB p_color); 
    void drawSide(CRGB* leds);
    void setColor(CRGB p_color);
  private:
    CRGB color;
    byte side_num;
    uint16_t start_idx;
    byte len;
};

#endif 
