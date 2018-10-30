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

            Color.CRGB[] palette_colors = {Color.CRGB.Aqua,Color.CRGB.Orange,
                Color.CRGB.BlueViolet, Color.CRGB.Coral,Color.CRGB.Yellow};

            Palette test = new Palette(palette_colors);

            Color[] testFrame = new Color[50 * 8];
            luc.sendFrame(sendSingleColorFrame(frame_size, new Color(Color.CRGB.Coral).bytes));
            Thread.Sleep(1000);
            luc.sendFrame(sendSingleColorFrame(frame_size, new Color(Color.CRGB.Black).bytes));
            Thread.Sleep(1000);

            //outputs the palette a few times onto the strips. 
            int pix = 0;
            for (int j = 0; j < testFrame.Length ;j++)
            {
                testFrame[j] = test.getColor(pix);
                pix += 10;
                if (pix > 255)
                    pix = 0;
            }

            luc.sendFrame(testFrame);

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
    }


}
