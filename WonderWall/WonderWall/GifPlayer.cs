using System;
using System.Drawing;
using System.Threading;

namespace WonderWall
{
    public class NetworkHelper
    {
        public const string UDP_HOST = "192.168.2.242";
        public const int UDP_PORT = 7777;
        public const int FRAME_SIZE = 273 * 8;

        public const int delay = 1000/24; //delay in ms

        private List<Bitmap> pixelFrames;

        public static void Main(string[] args)
        {
            NetworkHelper nh = new NetworkHelper(UDP_HOST, UDP_PORT);
            PixelsHelper ph = new PixelsHelper();
            Palette palette = new Palette();
            Color[] pixels = new Color[FRAME_SIZE];

            // Get the name of the gif file from the command line
            Image gifImage = Image.FromFile(path);
            FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);
            
            // Check that it's at least 24px wide and 91px tall
            // (same resolution as the grid)
            if (!(gifImage.Width >= 24 && gifImage.Height >= 91))
            {
                Console.WriteLine("Image has to be at least 24px wide by 91px high.");
                System.Environment.Exit(-1);
            }

            //For each frame in the gif
            for(int frameIdx = 0; frameIdx < frameCount; frameIdx++)
            {
                gifImage.SelectActiveFrame(dimension, frameIdx);
                Bitmap frame = (Bitmap)gifImage.Clone();
                Bitmap tmp;

                //get every width/24 column
                for(int x = 0; x < gifImage.Width; x += (int)(gifImage.Width/24))
                {
                    //get every width/94 row of that column
                    // TODO get rid of the magic resolution numbers
                    for (int y = 0; y < gifImage.Height; y += (int)(gifImage.Height/91))
                    {
                        tmp.SetPixel(x, y, frame.GetPixel(x, y));
                    }
                }
                
                pixelFrames.Add(tmp);
            }
            //While true
            while (true)
            {
                foreach(Bitmap data in pixelFrames)
                {
                    for(int x =0; x < data.Width; x++)
                    {
                        for (int y =0; y < data.Height; y++)
                        {
                            ph.SetOnePixel(data.GetPixel(x, y), ph.getPixelIdx(x,y), pixels);
                            nh.SendFrame(pixels);
                            Thread.Sleep(delay);
                        }
                    }
                }
            }
        }
    }
}
