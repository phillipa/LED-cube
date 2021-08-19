using System;
using System.Drawing;

namespace WonderWall
{
    /// <summary>
    /// utility functions to help set up array of pixels
    /// </summary>
    public class PixelsHelper
    {
        private int ROW_LEN = 91; //default 91 for LED tubes 

        public PixelsHelper(int p_ROW_LEN)
        {
                this.ROW_LEN = p_ROW_LEN; 
        }

        /// <summary>
        /// Set all the elements of pixels to a specified color 'c'
        /// </summary>
        /// <param name="c"></param>
        /// <param name="pixels"></param>
        public void SetAllPixels(Color c, ref Color[] pixels)
        {
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = c;
        }

        /// <summary>
        /// Set 1 pixel to color c in the pixels array
        /// </summary>
        /// <param name="c"></param>
        /// <param name="pidx"></param>
        /// <param name="pixels"></param>
        public void SetOnePixel(Color c, int pidx, ref Color[] pixels)
        {
            if (pidx > pixels.Length)
                return;

            //clear the pixels
            SetAllPixels(Color.Black, ref pixels);

            pixels[pidx] = c;

        }

        public void SetPixels(Color c, int start_idx, int end_idx, ref Color[] pixels)
        {
            for(int i = start_idx; i < end_idx && i < pixels.Length; i++)
                pixels[i] = c;
        }

        /// <summary>
        /// Fill pixels array with colors from a sequence
        ///
        /// start_idx tells it where in the sequence to start
        ///
        /// if sequence length is less than the number of pixels it loops 
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="start_idx"></param>
        /// <param name="pixels"></param>
        public void FillFromSequence(Color[] sequence, int start_idx, ref Color[] pixels)
        {
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = sequence[(i + start_idx) % sequence.Length];
        }


        /// <summary>
        /// 2D pixel plotting, deals with the fact that the the lines
        /// go back and forth
        /// pattern:
        /// FBF;FBF;FBF;FBF ...
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="clear_all"></param>
        /// <param name="pixels"></param>
        public void SetOnePixelWall(Color c, int x, int y, bool clear_all, ref Color[] pixels)
        {
            if (clear_all)
                SetAllPixels(Color.Black, ref pixels);

            int pixel_idx = GetPixelIdxWall(x, y);
            if (pixel_idx < pixels.Length)
                pixels[pixel_idx] = c;

        }

        /***
        * Function that maps an x/y coordinate into a linear 
        * coordinate, for the LED-wall (rows change direction 
        * every 3 rows)
        ***/
        private int GetPixelIdxWall(int x, int y)
        {
            int pixel_idx = y * ROW_LEN;

            //now need to figure out the direction this row goes
            if (y > 0 && (y - 1) % 3 == 0)
            {
                //reverse direction row
                pixel_idx += (ROW_LEN - x);
            }
            else
            {
                //forward direction row
                pixel_idx += x;
            }
            return pixel_idx;
        }

        public void FillFromCenter(Color[] palette, int rows, int columns, int start_idx, ref Color[] pixels)
        {
            double x_center = columns / 2;
            double y_center = rows / 2;

            //ph.FillFromSequence(palette.greens, i, ref pixels);
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int distance = (int)Math.Sqrt(Math.Pow(x - x_center, 2) + Math.Pow(y - y_center, 2));
                    int color_idx = (distance + start_idx) % palette.Length;

                    int pixel_idx = GetPixelIdxWall(x, y);

                    if (pixel_idx < pixels.Length)
                        pixels[pixel_idx] = palette[color_idx];
                }
            }

        }
    }
}
