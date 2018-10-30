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

        public Color getColor(int index)
        {
            return palette[index];
        }
    }
}
