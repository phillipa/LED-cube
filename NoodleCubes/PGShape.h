#include <FastLED.h>

class PGShape
{
  public:
    PGShape();
    void initShape(CRGB p_color, uint16_t* p_sides, uint16_t p_num_sides);
    void drawShape(CRGB* leds, uint16_t num_leds, byte side_len[]);
    void colorSide(CRGB* leds, uint16_t num_leds, byte side_len[], int side, CRGB p_color);
    void setColor(CRGB p_color);
  private:
    uint16_t sides[12]; //shape has max of 12 sides
    CRGB color;
    uint16_t num_sides;
  
};
