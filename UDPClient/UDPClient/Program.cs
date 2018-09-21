using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LEDUDPClient
{
    public static class CRGBColors
    {
        public static Byte[] BLACK = { 0x00, 0x00, 0x00 };
        public static Byte[] RED = { 0xcc, 0x00, 0x00 };
        public static Byte[] BLUE = { 0x00, 0x00, 0xcc };
        public static Byte[] GREEN = { 0x00, 0xcc, 0x00 };
        public static Byte[] PURPLE = { 0xcc, 0x00, 0xcc };
        public static Byte[] ORANGE = { 0xff, 0xa5, 0x00 };
        public static Byte[] AQUA = { 0x00, 0xff, 0xff };
    }

    class MainClass
    {
        public const string UDP_HOST = "192.168.2.1";
        public const int UDP_PORT = 7777;
        public const int FRAME_SIZE = 10;
        public static readonly Byte[] END = { 0x01, 0x00, 0x01 };   // clear
        public static readonly Byte[] START = { 0x01, 0x01, 0x00 }; // reset

        public static void LEDTestSequence(UdpClient client)
        {
            int delayBetweenTests = 1000;

            // START Test
            Console.WriteLine("Sending start sequence...");
            client.Send(START, START.Length);
            Thread.Sleep(delayBetweenTests);

            // END Test
            Console.WriteLine("Sending end sequence...");
            client.Send(END, END.Length);
            Thread.Sleep(delayBetweenTests);

            // Clear the frame
            Console.WriteLine("Wipe the whole frame...");
            Byte[] clearFrame = sendSingleColorFrame(FRAME_SIZE, CRGBColors.BLACK);
            client.Send(clearFrame, clearFrame.Length);
            Thread.Sleep(delayBetweenTests);

            // Purple
            Console.WriteLine("Purple...");
            Byte[] purples = sendSingleColorFrame(FRAME_SIZE, CRGBColors.PURPLE);
            client.Send(purples, purples.Length);
            Thread.Sleep(delayBetweenTests);

            Console.WriteLine("Done...");
            Console.ReadKey();
            return;
        }

        public static void Main(string[] args)
        {
            var udpClient = new UdpClient(UDP_HOST, UDP_PORT);

            try
            {               
                // TODO: Will use key input for controlling color
                //String input = Console.ReadLine();

                Console.WriteLine("Starting LED test sequence...");
                LEDTestSequence(udpClient);
                udpClient.Close();
                return;

                //for (int i = 0; i < (1000 * FRAME_SIZE); i++)
                //{
                //    if (i % 3 == 0)
                //        tosend = sendSinglePixelFrame(15 * 4, i % (15 * 4), purple);
                //    else if (i % 3 == 1)
                //        tosend = sendSinglePixelFrame(15 * 4, i % (15 * 4), orange);
                //    else if (i % 3 == 2)
                //        tosend = sendSinglePixelFrame(15 * 4, i % (15 * 4), aqua);

                //    udpClient.Send(tosend, tosend.Length, UDP_HOST, UDP_PORT);
                //    Thread.Sleep(20);

                //}
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with passing data to host: {e.Message}");
            }
        }

        /// <summary>
        /// Turns on a pixel at the pixelon-th position in a frame containing numpixels of LEDs
        /// </summary>
        /// <param name="numpixels"></param>
        /// <param name="pixelon"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Byte[] sendSinglePixelFrame(int numpixels, int pixelon, Byte[] color)
        {
            // Bytes = pixels + 2 for signaling * 3 for rgb
            Byte[] toreturn = new byte[(numpixels + 2) * 3]; 
            
            // Put reset to say new frame
            for (int i = 0; i < START.Length; i++)
                toreturn[i] = START[i];
            
            // Set it all to black 
            for (int i = START.Length; i < toreturn.Length; i++)
                toreturn[i] = 0;
            
            // Turn on the single pixel
            toreturn[pixelon * 3] = color[0];
            toreturn[(pixelon * 3) + 1] = color[1];
            toreturn[(pixelon * 3) + 2] = color[2];

            // Put the clear to tell the thing to display frame.
            for (int i = END.Length; i > 0; i--)
                toreturn[toreturn.Length-i] = END[END.Length-i];

            return toreturn;
        }
        
        /// <summary>
        /// Sets an entire frame of numpixels to a single color
        /// </summary>
        /// <param name="numpixels"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Byte[] sendSingleColorFrame(int numpixels, Byte[] color)
        {
            //bytes = pixels + 2 for signaling * 3 for rgb
            Byte[] toreturn = new byte[(numpixels + 2) * 3]; 
            
            //put reset to say new frame
            for (int i = 0; i < START.Length; i++)
                toreturn[i] = START[i];

            //pack trio of r/g/b for the color 
            for (int i = START.Length; i < toreturn.Length; i++)
                toreturn[i] = color[i % 3];

            //put the clear to tell the thing to display frame.
            for (int i = END.Length; i > 0; i--)
                toreturn[toreturn.Length - i] = END[END.Length - i];

            return toreturn;
        }

        /// <summary>
        /// Returns the next color in the color space
        /// 
        /// TODO: Need to split this into multiple methods, to handle different color spaces
        /// such as HSV
        /// </summary>
        /// <param name="currcolor"></param>
        /// <returns></returns>
        public int nextColor(int currcolor)
        {
            int toreturn = 0;

            byte r = (byte)(currcolor & 0x110000 >> 16);
            byte g = (byte)(currcolor & 0x001100 >> 8);
            byte b = (byte)(currcolor & 0x000011);

            //one color is set, increment it. 
            if (r > 0 && r < 255 && b == 0 && g == 0)
                r++;
            if (g > 0 && g < 255 && r == 0 && g == 0)
                g++;
            if (b > 0 && b < 255 && r == 0 && g == 0)
                b++;

            return (r << 16) | (g << 8) | (b);
        }
    }
}



