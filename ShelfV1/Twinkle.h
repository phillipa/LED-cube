
#include <FastLED.h>

class Twinkle
{
  public:
    Twinkle();
    void initTwinkle(uint8_t p_perc, uint8_t p_color);
    void updateTwinkle(bool randomize_colors);
    void setPerc(uint8_t p_perc);
    void drawTwinkle(CRGB* leds, int idx, CRGBPalette16 palette, TBlendType blending);
    
  private:
    uint8_t perc; //if pixel is off it comes alive with this prob. (out of 100)
    uint8_t color; //color is expecting to be an index into a palette for drawing
    uint8_t life; //ranges from 0-255 
};
