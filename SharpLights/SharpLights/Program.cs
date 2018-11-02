using System;
using System.Threading;

namespace SharpLights
{
   

    class MainClass
    {
        static int num_strips = 8;
        static int pixels_per_strip = 27;
        static int frame_size = num_strips * pixels_per_strip;

        static int frames_per_second = 60;//rough fps we are aiming for

        static Palette adbasic = null;
        static Palette rainbow = null;
        static Palette purplegreen = null;
        static Palette purples = null;
        static Palette blues = null;
        static Palette greens = null;

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LEDUDPClient luc = new LEDUDPClient("192.168.0.100", 7777, frame_size);

          
            //testing palette functions.

            initPalettes(10);

            Twinkle twinkle = new Twinkle(adbasic, true, 50, frame_size);

            luc.sendFrame(sendSingleColorFrame(frame_size, new Color(CRGB.AliceBlue)));
            Thread.Sleep(3000);
            //outputs the palette a few times onto the strips. 

            for (int i = 0; i < 500; i++)
            {
                luc.sendFrame(twinkle.getFrame());
                twinkle.iterate();
                Thread.Sleep(1000/frames_per_second);
            }

            luc.closeSocket();
        }

        /// <summary>
        /// assemble some of our old favorite palettes
        /// </summary>
        public static void initPalettes(int interval_length)
        {

            CRGB[] adbasic_colors = { CRGB.Purple,
                CRGB.Blue, CRGB.Chartreuse,
                CRGB.Green,CRGB.Purple, 
                CRGB.Blue,CRGB.DarkMagenta};

            adbasic = new Palette(adbasic_colors, interval_length);

            CRGB[] rainbow_colors = {CRGB.Red,
                CRGB.Orange, CRGB.Yellow, 
                CRGB.Green, CRGB.Blue,
                CRGB.Indigo,CRGB.Violet};

            rainbow = new Palette(rainbow_colors, interval_length);

            CRGB[] purplegreen_colors = {CRGB.DarkMagenta,
                CRGB.DarkOrchid, CRGB.DarkViolet, CRGB.MediumOrchid,
                CRGB.MediumPurple,CRGB.Purple, CRGB.BlueViolet,
                CRGB.Blue, CRGB.Blue, CRGB.Blue,
                CRGB.Chartreuse,CRGB.Green, CRGB.Chartreuse, CRGB.YellowGreen,
                CRGB.Lime,CRGB.Chartreuse};

            purplegreen = new Palette(purplegreen_colors, interval_length);

            CRGB[] purples_colors = { CRGB.DarkMagenta, CRGB.DarkOrchid,
                CRGB.DarkViolet,CRGB.BlueViolet};

            purples = new Palette(purples, interval_length);
           
            CRGB[] blues_colors = { CRGB.Blue, CRGB.Navy, CRGB.CornflowerBlue,
                CRGB.DodgerBlue,CRGB.SkyBlue,CRGB.DeepSkyBlue,
            CRGB.LightSkyBlue};

            blues = new Palette(blues, interval_length);

            CRGB[] greens_colors = {CRGB.LimeGreen, CRGB.Lime,
                CRGB.Green, CRGB.Chartreuse,CRGB.YellowGreen,
                CRGB.Chartreuse, CRGB.LimeGreen, CRGB.Lime};

            greens = new Palette(greens_colors, interval_length);
        }
        //for testing
        public static Byte[] sendSingleColorFrame(int numpixels, Byte[] color)
        {
            //bytes = pixels + 2 for signaling * 3 for rgb
            Byte[] toreturn = new byte[(numpixels) * 3];

            //pack trio of r/g/b for the color 
            for (int i = 0; i < toreturn.Length; i++)
                toreturn[i] = color[i % 3];

            return toreturn;
        }

        public static Color[] sendSingleColorFrame(int numpixels, Color color)
        {
            //bytes = pixels + 2 for signaling * 3 for rgb
            Color[] toreturn = new Color[(numpixels)];

            //pack trio of r/g/b for the color 
            for (int i = 0; i < toreturn.Length; i++)
                toreturn[i] = color;

            return toreturn;
        }

        //for testing
        public static Color[] sendPaletteFrame(int numpixels, Palette palette)
        {
            Color[] toreturn = new Color[numpixels];

            //pack trio of r/g/b for the color 
            for (int i = 0; i < toreturn.Length; i++)
            {
               
                toreturn[i] = palette.getColor(i%palette.palette_length);
            }

            //testing: make the first n pixels on strip n black
            for (int i = 0; i < num_strips;i++)
            {
                for (int j = 0; j <= i; j++)
                    toreturn[i * pixels_per_strip + j] = new Color(CRGB.White);
            }
            return toreturn;
        }
    }


}
