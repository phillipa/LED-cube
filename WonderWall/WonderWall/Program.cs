using System;
using System.Drawing;
using System.Threading;

namespace WonderWall
{
    class MainClass
    {

        //TODO how does wled do auto-discovery?
        public const string UDP_HOST = "192.168.2.127";
        public const int UDP_PORT = 21324;
        public const int FRAME_SIZE = 273 * 8;

        public const int delay = 1000/24; //delay in ms

        public static void Main(string[] args)
        {
            Console.WriteLine("Setting up pixel helper and network helper");
            NetworkHelperWLED nh = new NetworkHelperWLED(UDP_HOST, UDP_PORT);
            PixelsHelper ph = new PixelsHelper();
            Palette palette = new Palette();

            Color[] test_pattern = { Color.Aquamarine, Color.Chartreuse, Color.Chocolate, Color.HotPink };
            Color[] pixels = new Color[FRAME_SIZE];
            Color[] adbasic = palette.adbasic;

            try
            {
                int iter = 0;
                while (true)
                {
                    for(int x=0;x<91;x++)
                    {
                        for (int y = 0; y < 24; y++)
                            ph.SetOnePixel(adbasic[(y + iter) % adbasic.Length], x, y, false, ref pixels);
                    }
                    nh.Send(pixels);
                    Thread.Sleep(delay);

                    iter++;
                    iter %= pixels.Length;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with passing data to host: {e.Message}");
            }
            finally
            {
                nh.Close();
            }
        }
    }
}
