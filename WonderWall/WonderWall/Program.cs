using System;
using System.Drawing;
using System.Threading;

namespace WonderWall
{

    class MainClass
    {
    public enum Mode{
        Column,
        Cube,
        Wall
    }
        //TODO how does wled do auto-discovery?
        //Network params
        public const string UDP_HOST = "192.168.2.127";
        public const int UDP_PORT = 21324;

        //LED params
        public const int FRAME_SIZE = 273 * 8;
        public const int ROW_LEN = 91; //length of tube/row
        public const int FPS = 24;
        public const int FRAME_DELAY = 1000/FPS; //delay in ms

        //Which set of LEDs are we using
        public static Mode CurrentMode = Mode.Cube;



        public static void Main(string[] args)
        {
        //Get utilities set up + pixel array
        NetworkHelperWLED nh = new NetworkHelperWLED(UDP_HOST, UDP_PORT);
        PixelsHelper ph = new PixelsHelper(ROW_LEN);
        Palette palette = new Palette();
        Color[] pixels = new Color[FRAME_SIZE];

        Color[] currentPalette = palette.adbasic;
        int iter=0;
            try{ 
                while(true)
                {
                    //update the display based on the LED configuration
                    switch(CurrentMode)
                    {
                        case Mode.Column:
                            ColumnMode(ph,  currentPalette, pixels, iter);
                            break;
                        case Mode.Wall:
                            WallMode(ph,  currentPalette, pixels, iter);
                            break;
                        case Mode.Cube:
                            CubeMode(ph,  currentPalette, pixels, iter);
                            break;
                        default:
                            break;
                    }

                    iter++;

                    //send the frame
                    nh.Send(pixels);
                    Thread.Sleep(FRAME_DELAY);
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

        static void CubeMode(PixelsHelper ph, Color[] currentPalette, Color[] pixels, int iter){

                int side = 0;
                for(int i = 0; i < FRAME_SIZE; i+=91)
                {
                    ph.SetPixels(currentPalette[(iter) % currentPalette.Length], i, i+91, ref pixels);
                    side++;
                }
             
            
        }

        static void WallMode(PixelsHelper ph, Color[] currentPalette, Color[] pixels, int iter){
           
        }

        static void ColumnMode(PixelsHelper ph, Color[] currentPalette, Color[] pixels, int iter){
           
        }
    }
}
