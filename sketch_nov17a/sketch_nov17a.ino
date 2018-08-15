#define USE_OCTOWS2811
#include<OctoWS2811.h>
#include<FastLED.h>

#define NUM_LEDS_PER_STRIP 600 //546 
#define NUM_STRIPS 4
#define NUM_LEDS 2400 //1092 //4*91
CRGB leds[NUM_STRIPS * NUM_LEDS_PER_STRIP];
#define BRIGHTNESS  128

CRGBPalette16 currentPalette;
TBlendType    currentBlending;

int global_i = 0;
boolean up = true;

// Pin layouts on the teensy 3:
// OctoWS2811: 2,14,7,8,6,20,21,5

void setup() {
    randomSeed(analogRead(0));
  randomSeed(analogRead(random(0, 7)));
  
  LEDS.addLeds<OCTOWS2811>(leds, NUM_LEDS_PER_STRIP);
  LEDS.setBrightness(BRIGHTNESS);
for (int i = 0; i < NUM_LEDS;i++)
    leds[i] = 0x00ffff;

    LEDS.delay(1000);
for (int i = 0; i < NUM_LEDS;i++)
    leds[i] = 0x000000;

currentPalette = HeatColors_p;//PartyColors_p;//RainbowColors_p;
    currentBlending = LINEARBLEND;
          FillLEDsFromPaletteColors( 0);

    }

void FillLEDsFromPaletteColors( uint8_t colorIndex)
{
    uint8_t brightness = 255;
    
    for( int i = 0; i < NUM_LEDS; i++) {
        leds[i] = ColorFromPalette( currentPalette, (colorIndex+i)%255, brightness, currentBlending);
    }
}

int g_regen = 50;
void loop() {

//undulating
/*for(int i=0; i < 600;i++)
{
  FillLEDsFromPaletteColors(i);
     LEDS.show();
    LEDS.delay(100);
}*/

CRGBPalette16 palettes[]={HeatColors_p,PartyColors_p,CloudColors_p,RainbowColors_p};
for(int p = 0; p < 4; p++)
{
  init_twinkle(75);
for(int i = 0; i <255-50;i++)
{
  for(int t = 0; t < 50; t++)
  {
   // game_of_life(g_regen,ColorFromPalette( palettes[p], i, 128, currentBlending));
  currentPalette=palettes[p];
   game_of_life_random_from_palette(0,i,50);//2);//g_regen);
        LEDS.show();
    LEDS.delay(200);  
  }

}
}
g_regen-=5;
if(g_regen<=0)
  g_regen=50;

}

void init_twinkle(int start_perc)
{
   for(int i = 0 ; i < NUM_LEDS; i++)
  {
    leds[i] = CRGB::Black;
  
  CRGB live_color= ColorFromPalette( currentPalette, i%255, 255, currentBlending);//live_color;
    int r1 = random(0,100);
      if((r1 < start_perc) ) //3% come to life (if dead)
      {
        leds[i] = live_color;//ColorFromPalette( currentPalette, i, 255, currentBlending);//live_color;
      }
}
}
 void randomfade()
{
   uint8_t brightness = 255;
    
    for( int i = 0; i < NUM_LEDS; i++) {
      if(random(0,100)<25)
      {
        leds[i].r>>=2;
        leds[i].g>>=2;
        leds[i].b>>=2;
      }   
      else
      {
        if(leds[i].r==0 && leds[i].g==0 && leds[i].b==0)
        leds[i]= ColorFromPalette( currentPalette, i, brightness, currentBlending);
      }
     }
}

void fade_pixel(int i)
{
   int fr = random(0,2);
          leds[i].r=leds[i].r >>fr;
          leds[i].g=leds[i].g >>fr;
          leds[i].b=leds[i].b >>fr;
}
void game_of_life_full_palette(int regen)
{
  
 // int regen = 5; //percent that will come to life
     CRGB last_leds[NUM_STRIPS * NUM_LEDS_PER_STRIP];
  for(int i = 0 ; i < NUM_LEDS; i++)
    last_leds[i] = leds[i];
    
    for(int i = 0 ; i < NUM_LEDS; i++)
  {
CRGB live_color= ColorFromPalette( currentPalette, i%255, 255, currentBlending);//live_color;
    int r1 = random(0,100);
      if((r1 < regen) && ((int)last_leds[i] == 0x000000)) //3% come to life (if dead)
      {
        leds[i] = live_color;//ColorFromPalette( currentPalette, i, 255, currentBlending);//live_color;
      }
    else
    {
      int live_neighbors = 0;
      
      if(i > 0 && ((int)last_leds[i-1] > 0)) //adding minimum liveliness
        live_neighbors+= (int)last_leds[i-1];
      if(i < NUM_LEDS && ((int)last_leds[i+1] >0))
        live_neighbors+=(int)last_leds[i+1];
      if((int)last_leds[i] == 0x000000)
      {
        if(live_neighbors>0)//(int)(live_neighbors/2) >= (int)(live_color)) // sum of neighbors is enough
        {  leds[i] = live_color;
          int fadeval=random(0,3);
          leds[i].r >>= fadeval;
          leds[i].g >>= fadeval;
          leds[i].b >>= fadeval;
        }
        
      }
      else
      {
        //fade
       fade_pixel(i);
      }
    }
  }
}

void game_of_life_random_from_palette(int regen, int start_color, int num_colors)
{
  
 // int regen = 5; //percent that will come to life
     CRGB last_leds[NUM_STRIPS * NUM_LEDS_PER_STRIP];
  for(int i = 0 ; i < NUM_LEDS; i++)
    last_leds[i] = leds[i];
    
    for(int i = 0 ; i < NUM_LEDS; i++)
  {
CRGB live_color= ColorFromPalette( currentPalette, random(start_color,start_color+num_colors)%255, 255, currentBlending);//live_color;
    int r1 = random(0,100);
      if((r1 < regen) && ((int)last_leds[i] == 0x000000)) //3% come to life (if dead)
      {
        leds[i] = live_color;//ColorFromPalette( currentPalette, i, 255, currentBlending);//live_color;
      }
    else
    {
      int live_neighbors = 0;
      
      if(i > 0 && ((int)last_leds[i-1] > 0)) //adding minimum liveliness
        live_neighbors+= (int)last_leds[i-1];
      if(i < NUM_LEDS && ((int)last_leds[i+1] >0))
        live_neighbors+=(int)last_leds[i+1];
      if((int)last_leds[i] == 0x000000)
      {
         if(live_neighbors>0)//(int)(live_neighbors/2) >= (int)(live_color)) // sum of neighbors is enough
        {  leds[i] = live_color;
          int fadeval=random(0,3);
          leds[i].r >>= fadeval;
          leds[i].g >>= fadeval;
          leds[i].b >>= fadeval;
        }
      }
      else
      {
        //fade
       // if(random(0,10)<6)
            fade_pixel(i);

      }
    }
  }
}

void game_of_life(int regen, CRGB l)
{
  
 // int regen = 5; //percent that will come to life
     CRGB last_leds[NUM_STRIPS * NUM_LEDS_PER_STRIP];
  for(int i = 0 ; i < NUM_LEDS; i++)
    last_leds[i] = leds[i];
    
    for(int i = 0 ; i < NUM_LEDS; i++)
  {
CRGB live_color= l;//ColorFromPalette( currentPalette, i, 255, currentBlending);//live_color;
    int r1 = random(0,100);
      if((r1 < regen) && ((int)last_leds[i] == 0x000000)) //3% come to life (if dead)
      {
        leds[i] = live_color;//ColorFromPalette( currentPalette, i, 255, currentBlending);//live_color;
      }
    else
    {
      int live_neighbors = 0;
      
      if(i > 0 && ((int)last_leds[i-1] > 0)) //adding minimum liveliness
        live_neighbors+= (int)last_leds[i-1];
      if(i < NUM_LEDS && ((int)last_leds[i+1] >0))
        live_neighbors+=(int)last_leds[i+1];
      if((int)last_leds[i] == 0x000000)
      {
        if(live_neighbors > (int)live_color) // sum of neighbors is enough
          leds[i] = live_color;
        
      }
      else
      {
        //fade
             fade_pixel(i);

      }
    }
  }
}





void diagonals(int gap)
{
   
  
for (int i = 0; i < NUM_LEDS;i++)
{
    int curr_pixel = (i+global_i)%NUM_LEDS;
    if(i % gap == 0)
        leds[curr_pixel] = 0xffffff;
     else
     {
      if((int)leds[curr_pixel] != 0x000000)
      {
        leds[curr_pixel].r=leds[(i+global_i)%NUM_LEDS].r >>1;
        leds[curr_pixel].g=leds[(i+global_i)%NUM_LEDS].g >>1;
        leds[curr_pixel].b=leds[(i+global_i)%NUM_LEDS].b >>1;
      }
     }

  
} 

global_i++;
global_i %= NUM_LEDS;
}




void twinkle()
{
  for(int i =0; i < NUM_LEDS; i++)
  {
    if((int)leds[i] == 0x000000)
  {
    if(random(1,100)<33)
      leds[i] = 0xffffff;
  }
  else
  {
    int r = random(0,2);
    int r2 = random(0,10);
    if(r2 < 5)
    {
    leds[i].r = leds[i].r >> r;
    leds[i].g = leds[i].g >> r;
    leds[i].b = leds[i].b >> r;
    }
    else
    {
    leds[i].r = leds[i].r << r;
    leds[i].g = leds[i].g << r;
    leds[i].b = leds[i].b << r;
    }
  }
  
}
}

