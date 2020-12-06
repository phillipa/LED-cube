#include <Adafruit_NeoPXL8.h>

/* The NeoPXL8 has 8 outputs, call them 0-7 sequentially. 
 * 0 - Sck (default) or RX
 * 1 - pin 5 (default) or TX
 * 2 - pin 9 (default) or SCL 
 * 3 - pin 6 (default) or SDA
 * 4 - pin 13
 * 5 - pin 12
 * 6 - pin 11
 * 7 - pin 10
 * If you change the pins array, you also have to mod the hardware to match
 */
int8_t pins[8] = { 13, 12, 11, 10, SCK, 5, 9, 6 };

#define NUM_LED 600
#define NUM_ROWS 12
#define ROW_LEN 52
// Args are number of LEDs (per strip), pin assignments, color order/type
Adafruit_NeoPXL8 leds(NUM_LED, pins, NEO_GRB);

void setup() {
  leds.begin();
  leds.setBrightness(32);
}

uint8_t frame = 0;

void loop() {
  for(uint8_t r=0; r<NUM_ROWS; r++) { // For each row...
    for(int p=0; p<ROW_LEN; p++) { // For each pixel of row...
      leds.setPixelColor(r * NUM_LED + p, rain(r, p));
    }
  }
  leds.show();
  frame++;
}

uint8_t colors[8][3] = { // RGB colors for the 8 rows...
  255,   0,   0, // Row 0: Red
  255, 160,   0, // Row 1: Orange
  255, 255,   0, // Row 2: Yellow
    0, 255,   0, // Row 3: Green
    0, 255, 255, // Row 4: Cyan
    0,   0, 255, // Row 5: Blue
  192,   0, 255, // Row 6: Purple
  255,   0, 255  // Row 7: Magenta
};

// Gamma-correction table improves the appearance of midrange colors
#define _GAMMA_ 2.6
const int _GBASE_ = __COUNTER__ + 1; // Index of 1st __COUNTER__ ref below
#define _G1_ (uint8_t)(pow((__COUNTER__ - _GBASE_) / 255.0, _GAMMA_) * 255.0 + 0.5),
#define _G2_ _G1_ _G1_ _G1_ _G1_ _G1_ _G1_ _G1_ _G1_ // Expands to 8 lines
#define _G3_ _G2_ _G2_ _G2_ _G2_ _G2_ _G2_ _G2_ _G2_ // Expands to 64 lines
const uint8_t gamma8[] = { _G3_ _G3_ _G3_ _G3_ };    // 256 lines

// Given row number (0-7) and pixel number along row (0 - (NUM_LED-1)),
// first calculate brightness (b) of pixel, then multiply row color by
// this and run it through gamma-correction table.
uint32_t rain(uint8_t row, int pixelNum) {
  uint16_t b = 256 - ((frame - row * 32 + pixelNum * 256 / NUM_LED) & 0xFF);
  return ((uint32_t)gamma8[(colors[row][0] * b) >> 8] << 16) |
         ((uint32_t)gamma8[(colors[row][1] * b) >> 8] <<  8) |
                    gamma8[(colors[row][2] * b) >> 8];
}
