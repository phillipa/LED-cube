using System;

namespace SharpLights
{

   

    /// <summary>
    /// Palette.
    /// Basically seems to work. 
    /// 
    /// Given an Array of Colors, create a palette blending between them 
    /// </summary>
    public class Palette
    {



        public Color[] palette;
        public int palette_length;
        //Create a palette given the input colors
        //it will blend between colors in the specified number of steps
        //ie., "steps" colors between each color in the input array 
        public Palette(CRGB[] colors, int steps)
        {
            //blends the last color back into the first color
            palette = new Color[ (colors.Length) * steps];
            palette_length =  (colors.Length) * steps;

            for (int c = 0; c < colors.Length;c++)
            {
                Color color1 = new Color(colors[c]);
                Color color2 = new Color(colors[(c+1)%colors.Length]);
                for (int i = 0; i < steps; i++)
                {
                    double blend_perc = 100* i / steps;
                    palette[c * steps + i] = Color.GetBlendedColor(color1, color2, (int)blend_perc);

                }
            }
        }

        /// <summary>
        /// Gets a random palette.
        /// you can specify how many colors and how much blending you want 
        /// between them (larger interval ==> more steps between colors)
        /// </summary>
        /// <returns>The random palette.</returns>
        /// <param name="num_colors">Number colors.</param>
        /// <param name="interval">Interval.</param>
       public static Palette getRandomPalette(int num_colors, int interval)
        {
            Palette toreturn;
            
            CRGB[] palette_colors = new CRGB[num_colors];

            for (int i = 0; i < palette_colors.Length;i++)
                palette_colors[i] = Color.getRandomCRGBColor();


            toreturn = new Palette(palette_colors, interval);
            return toreturn;
        }

        //if index is too big return the wrapped around value 
        public Color getColor(int index)
        {
            return palette[index%palette_length];
        }

        public Color getRandomColor()
        {

            Random r = new Random((int)(DateTime.Now.Ticks));
            return palette[r.Next(0,palette_length)];
        }
    }
}
