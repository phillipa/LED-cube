using System;
namespace SharpLights
{
    /// <summary>
    /// Palette.
    /// Basically seems to work. 
    /// 
    /// Given an Array of Colors, create a palette of 256 colors, 
    /// blend between colors in the input array. 
    /// </summary>
    public class Palette
    {

        Color[] palette = new Color[256];

        //Create a palette of 256 colors out of the array of colors
        //Blend between the colors 
        public Palette(CRGB[] colors)
        {
            //how many colors between colors given to us (interval length)
            int i_colors = (256) / (colors.Length-1);
            int num_intervals = colors.Length-1;

            for (int c = 0; c < num_intervals; c++)
            {
                //colors at start and end of this interval
                Color color1 = new Color(colors[c]);
                Color color2 = new Color(colors[c+1]);
                for (int i = 0; i < i_colors; i++)
                {
                    double blend_perc = 100 * ((double)i / (double)i_colors);
                    //blend with percentages going from 1/# intervals --> 100%
                    palette[(c*i_colors)+i] = 
                        Color.GetBlendedColor(color1, color2, (int)blend_perc);
                }
            }
            //add the last color in.
            palette[palette.Length - 1] = new Color(colors[colors.Length - 1]);
        }

        public Color getColor(int index)
        {
            return palette[index];
        }
    }
}
