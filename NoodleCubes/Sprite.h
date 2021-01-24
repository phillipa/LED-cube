
#include <FastLED.h>

#define FORWARD 0
#define BACKWARD 1

class Sprite
{
  public:
    Sprite();
    void initSprite(CRGB p_color, uint8_t p_speed, uint16_t p_loc, byte p_dir);
    void updateSprite();
    void drawSprite(CRGB* leds,  uint16_t num_leds);
    void setDirChangePerc(uint8_t perc);
    void setColor(CRGB p_color);
    void turnOnAI();
    void turnOffAI();
  private:
    CRGB color; 
    uint8_t speed; //ranges from 0-255 
    uint16_t loc;
    uint8_t dcp; //direction change probability 
    byte dir;
    bool ai_on;
    CRGBPalette16 pal;    

};
