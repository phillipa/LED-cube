using System;
using System.Drawing;
using System.Threading;
using System.Windows.Media.Imaging.BitmapSource;

namespace GIFLoader
{

    class MainClass
    {
        // Network params
    public
        const string UDP_HOST = "192.168.2.127";
    public
        const int UDP_PORT = 21324;

        // LED params
    public
        const int FRAME_SIZE = 273 * 8;
    public
        const int ROW_LEN = 91; // length of tube/row
    public
        const int FPS = 24;
    public
        const int FRAME_DELAY = 1000 / FPS; // delay in ms

    public
        static void Main(string[] args)
        {
            // Get utilities set up + pixel array
            NetworkHelperWLED nh = new NetworkHelperWLED(UDP_HOST, UDP_PORT);
            PixelsHelper ph = new PixelsHelper(ROW_LEN);
            Color[] pixels = new Color[FRAME_SIZE];

            //Load an animated gif
            BitmapDecoder uriBitmap = BitmapDecoder.Create(
                new Uri("/home/ams/Pictures/anim_diamond.gif", UriKind.Absolute),
                        BitmapCreateOptions.None,
                        BitmapCacheOption.Default);
            
            //Decimate the frames to the size of the column and convert to pixel arrays
            int column_w = 51;
            int column_h = 72;
            int img_w = uriBitmap.Frames[0].PixelWidth;
            int img_h = uriBitmap.Frames[0].PixelHeight;

            //The clever approach is to do a bicubic interpolation as it downsamples or something,
            //this is a straight up decimation of the image
            int w_stride = img_w/column_w;
            int h_stride = img_h/column_h;

            //GIF becomes list of arrays of colors
            List<Color[column_w * column_h]> deci_frames;

            foreach (BitmapFrame gif_frame in uriBitmap.Frames)
            {
                // I think this is a memory leak if the gif gets loaded over and over...
                Color[] tmp_frame = new Color[column_w * column_h];
                int idx = 0;

                for(int xx = 0; xx < column_w; xx += w_stride)
                {
                    for(int yy = 0; yy < column_h; yy += h_stride)
                    {
                        CroppedBitmap cb = new CroppedBitmap(gif_frame, new Int32Rect(xx, yy, 1, 1));
                        var pixels = new byte[4];
                        cb.CopyPixels(pixels, 4, 0);
                        tmp_frame[idx] = Color.FromRgb(pixels[2], pixels[1], pixels[0]);    
                        idx++;
                    }
                }

                // Is this a copy or a reference? 
                deci_frames.Add(tmp_frame);
            }

            int iter = 0;
            try
            {
                while (true)
                {
                    //Bounds check the index here
                    if(iter >= deci_frames.Length){
                        iter = 0;
                    }

                    // Send the frame
                    nh.Send(deci_frames[iter]));
                    Thread.Sleep(FRAME_DELAY);
                    iter++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($ "Error with passing data to host: {e.Message}");
            }
            finally
            {
                nh.Close();
            }
        }
    }
}
