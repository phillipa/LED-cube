using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;

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
        public const string UDP_HOST = "192.168.0.18";
        public const int UDP_PORT = 7777;
        public const int FRAME_SIZE = 27*8;
        public static readonly Byte[] END = { 0x01, 0x00, 0x01 };   // clear
        public static readonly Byte[] START = { 0x01, 0x01, 0x00 }; // reset

        private static void WaitResponse(int delay, bool waitUser)
        {
            Thread.Sleep(delay);
            if (waitUser) Console.ReadKey();
        }

        public static void LEDTestSequence(UdpClient client)
        {
            Console.WriteLine($"UDP Host: {UDP_HOST}");
            Console.WriteLine($"UDP Port: {UDP_PORT}");
            Console.WriteLine($"Frame Size: {FRAME_SIZE}");

            var sw = new Stopwatch();

            // TODO: Create a delegate that uses WaitResponse as a callback whenever UDPClient.Send is invoked

            // Clear the frame
            Console.WriteLine("Wipe the whole frame...");
            Byte[] clearFrame = sendSingleColorFrame(FRAME_SIZE, CRGBColors.BLACK);
            client.Send(clearFrame, clearFrame.Length);
            WaitResponse(1000, false);

            // Purple
            Console.WriteLine("Purple...");
            Byte[] purples = sendSingleColorFrame(FRAME_SIZE, CRGBColors.PURPLE);
            client.Send(purples, purples.Length);
            WaitResponse(1000, false);

            //Moving pixel
            Console.WriteLine("Moving pixel...");
            client.Send(clearFrame, clearFrame.Length); // Clear frame first
            Byte[] moveSinglePixelFrame;
            for (int i = 1; i <= FRAME_SIZE; i++)
            {
                moveSinglePixelFrame = sendSinglePixelFrame(FRAME_SIZE, i, CRGBColors.RED);
                client.Send(moveSinglePixelFrame, moveSinglePixelFrame.Length);
                Thread.Sleep(20);
            }
            WaitResponse(1000, false);

            Console.WriteLine("Pixel write test...");
            client.Send(clearFrame, clearFrame.Length); // Clear frame first
            Byte[] shiftSinglePixelFrame;
            int iterations = 5000;
            int pixelSleep = 0;

            //TODO: Introduce notion of frame sequences
            int frameSequenceLength = 60;
            sw.Start();
            for (int i = 1; i <= (iterations * frameSequenceLength); i++)
            {
                if (i % 3 == 0)
                    shiftSinglePixelFrame = sendSinglePixelFrame(FRAME_SIZE, i % (FRAME_SIZE), CRGBColors.PURPLE);
                else if (i % 3 == 1)
                    shiftSinglePixelFrame = sendSinglePixelFrame(FRAME_SIZE, i % (FRAME_SIZE), CRGBColors.ORANGE);
                else
                    shiftSinglePixelFrame = sendSinglePixelFrame(FRAME_SIZE, i % (FRAME_SIZE), CRGBColors.AQUA);

                // TODO: UDP frame size may be different from LED frame
                // LED frames might be large enought to require multiple UDP frames
                client.Send(shiftSinglePixelFrame, shiftSinglePixelFrame.Length);
                //Thread.Sleep(pixelSleep);
            }
            sw.Stop();
            Helpers.WriteLEDBenchmarks(FRAME_SIZE, frameSequenceLength, iterations, pixelSleep, sw.Elapsed);
            Console.WriteLine("Done...");
            WaitResponse(1000, true);
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
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with passing data to host: {e.Message}");
            }
            finally
            {
                udpClient.Close();
            }
        }

        /// <summary>
        /// Turns on a pixel at the pixelon-th position in a frame containing numpixels of LEDs
        /// 
        /// Pixelon is currently 1-indexed
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
                toreturn[toreturn.Length - i] = END[END.Length - i];

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



