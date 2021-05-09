using System;
using System.Drawing;
using System.Threading;

namespace WonderWall
{
    class MainClass
    {
        public const string UDP_HOST = "192.168.2.242";
        public const int UDP_PORT = 7777;
        public const int FRAME_SIZE = 273 * 8;

        public const int delay = 1000/24; //delay in ms

        public static void Main(string[] args)
        {
            Console.WriteLine("Setting up pixel helper and network helper");
            NetworkHelper nh = new NetworkHelper(UDP_HOST, UDP_PORT);
            PixelsHelper ph = new PixelsHelper();
            Palette palette = new Palette();

            Color[] test_pattern = { Color.Aquamarine, Color.Chartreuse, Color.Chocolate, Color.HotPink };
            Color[] pixels = new Color[FRAME_SIZE];
            Color[] adbasic = palette.adbasic;

            try
            {
                int iter = 0;
                int mod = 4;
                while (true)
                {
                    //ph.SetOnePixel(adbasic[iter%adbasic.Length], iter % 91, iter % 11, false, ref pixels);
                    //ph.FillFromCenter(adbasic, 24, 91, iter, ref pixels);

                    /*for(int i =0;i<pixels.Length;i++)
                    {
                        if ((i % 4) == 0)
                            ph.SetOnePixel(adbasic[i%adbasic.Length], i, ref pixels);
                    }*/

                    for(int x=0;x<91;x++)
                    {
                        for (int y = 0; y < 24; y++)
                            ph.SetOnePixel(adbasic[(y + iter) % adbasic.Length], x, y, false, ref pixels);
                    }
                    nh.SendFrame(pixels);
                    Thread.Sleep(delay);

                    /** undulating palette
                    ph.FillFromSequence(adbasic, iter % adbasic.Length, ref pixels);

                    nh.SendFrame(pixels);
                    Thread.Sleep(delay);
                    */

                    /** Pixels on mod 'mod'
                    ph.SetAllPixels(Color.Black, ref pixels);
                    for(int x = 0; x < 91; x++)
                    {
                        for(int y = 0; y < 24; y++)
                        {
                            if((iter+ph.GetPixelIdx(x,y)) % mod ==0)
                                ph.SetOnePixel(adbasic[iter % adbasic.Length], x, y, false, ref pixels);
                        }
                    }

                    nh.SendFrame(pixels);
                    Thread.Sleep(delay);

                    if(iter%24==0)
                    {
                        mod++;
                        if(mod == 10)
                            mod = 4;
                    }
                    */

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
