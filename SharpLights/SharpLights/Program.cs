using System;
using System.Threading;

namespace SharpLights
{
   

    class MainClass
    {
        static int num_strips = 8;
        static int pixels_per_strip = 27;
        static int frame_size = num_strips * pixels_per_strip;

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LEDUDPClient luc = new LEDUDPClient("192.168.0.18", 7777, frame_size);

          
            //testing palette functions.

            Color.CRGB[] palette_colors = {Color.CRGB.Red,Color.CRGB.Green,Color.CRGB.Blue,Color.CRGB.Purple};

            Palette test = new Palette(palette_colors);

            Color[] testFrame = new Color[50 * 8];
            luc.sendFrame(sendSingleColorFrame(frame_size, new Color(Color.CRGB.Coral)));
            Thread.Sleep(5000);
            luc.sendFrame(sendSingleColorFrame(frame_size, new Color(Color.CRGB.Black).bytes));
            Thread.Sleep(1000);

            //outputs the palette a few times onto the strips. 
        
            luc.sendFrame(sendPaletteFrame(frame_size,test));

            luc.closeSocket();
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
               
                toreturn[i] = palette.getColor(i%256);
            }

            return toreturn;
        }
    }


}
