#include "Sprite.h"

//empty default constructor
Sprite::Sprite()
{
  
}

void Sprite::initSprite(CRGB p_color, uint8_t p_speed, uint16_t p_loc, byte p_dir)
{
  color = p_color; 
  speed = p_speed;
  loc = p_loc; 
  dir = p_dir;

  dcp = 0; // default they won't change directions
  ai_on = false;
  
  pal = CRGBPalette16( color, CRGB::Black, CRGB::Black, CRGB::Black, 
  CRGB::Black, CRGB::Black, CRGB::Black, CRGB::Black, 
  CRGB::Black, CRGB::Black, CRGB::Black, CRGB::Black, 
  CRGB::Black, CRGB::Black, CRGB::Black, CRGB::Black );
 
}

void Sprite::setColor(CRGB p_color)
{
  color = p_color; 

  pal = CRGBPalette16( color, CRGB::Black, CRGB::Black, CRGB::Black, 
  CRGB::Black, CRGB::Black, CRGB::Black, CRGB::Black, 
  CRGB::Black, CRGB::Black, CRGB::Black, CRGB::Black, 
  CRGB::Black, CRGB::Black, CRGB::Black, CRGB::Black );
}

//takes a # 0-100 which gives the percent of the time 
//sprite will change directions 
void Sprite::setDirChangePerc(uint8_t perc)
{
  dcp = perc;
}

void Sprite::turnOnAI()
{
  ai_on = true;
}

void Sprite::turnOffAI()
{
  ai_on = false;
}

void Sprite::updateSprite()
{
    if(ai_on)
    {
     int max_speed = 3;
     int min_speed = 1;
     
  
      if(dir == BACKWARD && loc%9==0)
      {
        speed--;
        if(speed == min_speed)
        {
          speed = max_speed;
        }
        
      }
    }
    
    if(dir == FORWARD)
      loc += speed;
    else
      loc -= speed;
  
    if(random(0,100) < dcp)
    {
      if(dir == FORWARD)
        dir = BACKWARD;
      else
        dir = FORWARD;
    }
  

}



void Sprite::drawSprite(CRGB* leds,  uint16_t num_leds)
{
  loc %= num_leds; //contain the sprite in the LEDs array
  
  leds[loc] += ColorFromPalette(pal, 0);
  
  uint16_t t_loc = loc-1; //tail location
  if(dir == BACKWARD)
  t_loc = loc+1;
  
  uint8_t tail_len = 16;
  
  while(tail_len > 1)
  {
  
    if(t_loc >=0 && t_loc < num_leds)
    {  
      leds[t_loc] += ColorFromPalette(pal, 4*(16-tail_len));
    }
    tail_len--;
    if(dir == FORWARD)
      t_loc--; 
    else 
      t_loc++;
  }

}
    
