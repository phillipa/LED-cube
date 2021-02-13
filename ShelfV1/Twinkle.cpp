#include "Twinkle.h"

//empty default constructor
Twinkle::Twinkle()
{
  
}

/**
 * Init the twinkles with the probability that the twinkle is alive
 * and the color it should be (color is a 0-255 index into a color palette
 * that is specified later).
 * 
 * Twinkle uses palette indexes b/c it uses the brightness parameter 
 * in ColorFromPalette to fade out. 
 */
void Twinkle::initTwinkle(uint8_t p_perc, uint8_t p_color)
{
  perc = p_perc;
  color = p_color;
  
  if(random(0,100) < perc)
    life = random(0, 256);
  else
    life = 0;
}

void Twinkle::updateTwinkle(bool randomize_colors)
{
  if(life > 0)
    life = life >> 1;
  else if(random(0,100) < perc)
  {
    if(randomize_colors)
      color = random(0,255);
    life = 255;
  }
}

void Twinkle::setPerc(uint8_t p_perc)
{
  perc = p_perc;
}

void Twinkle::drawTwinkle(CRGB* leds, int idx, CRGBPalette16 palette, TBlendType blending)
{
  if(life > 0)//only color live pixels
    leds[idx] = ColorFromPalette(palette, color, life, blending);
}



