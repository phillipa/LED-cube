using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LEDUDPClient
{


    class MainClass
    {


        public static void Main(string[] args)
        {
              UdpClient udpClient = new UdpClient();
            try
            {

                //  String input = Console.ReadLine();
                //  while (input != "q")
                //  {


                Byte[] red = { 0xcc, 0x00, 0x00 };
                Byte[] blue = { 0x00, 0x00, 0xcc };
                Byte[] green = { 0x00, 0xcc, 0x00 };
                Byte[] purple = { 0xcc, 0x00, 0xcc };
                Byte[] orange = { 0xff, 0xa5, 0x00 };
                Byte[] aqua = { 0x00, 0xff, 0xff };

                Byte[] clearBytes = { 0x01, 0x00, 0x01 };
                Byte[] rstBytes = { 0x01, 0x01, 0x00 };

                Byte[] sendBytes = { 0xcc, 0x00, 0xcc };

                /*  if (input == "r")
                      sendBytes = red;
                  else if (input == "g")
                      sendBytes = green;
                  else if (input == "b")
                      sendBytes = blue;
                  else
                      sendBytes = purple;*/


               // udpClient.Send(rstBytes, rstBytes.Length, "192.168.0.27", 7777);

                Byte[] tosend = sendSingleColorFrame(300, purple);
                for (int i = 0; i < (1000*15); i++)
                {
                    if(i%3==0)
                    tosend = sendSinglePixelFrame(15 * 4, i%(15*4), purple);
                    else if (i%3 ==1)
                    tosend = sendSinglePixelFrame(15 * 4, i % (15 * 4), orange);
                    else if(i%3 ==2)
                    tosend = sendSinglePixelFrame(15 * 4, i % (15 * 4), aqua);

                    udpClient.Send(tosend, tosend.Length, "192.168.0.27", 7777);
                    Thread.Sleep(20);
                   
                }
                //udpClient.Send(clearBytes, clearBytes.Length, "192.168.0.27", 7777);


                udpClient.Close();
            }
            catch (Exception e) { }



        }

        public static Byte[] sendSinglePixelFrame(int numpixels, int pixelon,Byte[] color)
        {
            Byte[] toreturn = new byte[(numpixels + 2) * 3]; //bytes = pixels + 2 for signaling * 3 for rgb
            Byte[] clearBytes = { 0x01, 0x00, 0x01 };
            Byte[] rstBytes = { 0x01, 0x01, 0x00 };
            //put reset to say new frame
            for (int i = 0; i < rstBytes.Length; i++)
                toreturn[i] = rstBytes[i];
            //set it all to black 
            for (int i = rstBytes.Length; i < toreturn.Length; i++)
                toreturn[i] = 0;
            //turn on the single pixel
            toreturn[pixelon * 3] = color[0];
            toreturn[(pixelon * 3) + 1] = color[1];
            toreturn[(pixelon * 3) + 2] = color[2];
            //put the clear to tell the thing to display frame.
            toreturn[toreturn.Length - 3] = clearBytes[0];
            toreturn[toreturn.Length - 2] = clearBytes[1];
            toreturn[toreturn.Length - 1] = clearBytes[2];

            return toreturn;
        }

        public static Byte[] sendSingleColorFrame(int numpixels, Byte[] color)
        {
            Byte[] toreturn = new byte[(numpixels+2)*3]; //bytes = pixels + 2 for signaling * 3 for rgb
            Byte[] clearBytes = { 0x01, 0x00, 0x01 };
            Byte[] rstBytes = { 0x01, 0x01, 0x00 };
            //put reset to say new frame
            for (int i = 0; i < rstBytes.Length; i++)
                toreturn[i] = rstBytes[i];
            //pack trio of r/g/b for the color 
            for (int i = rstBytes.Length; i < toreturn.Length;i++)
                toreturn[i] = color[i % 3];
            //put the clear to tell the thing to display frame.
            toreturn[toreturn.Length - 3] = clearBytes[0];
            toreturn[toreturn.Length - 2] = clearBytes[1];
            toreturn[toreturn.Length - 1] = clearBytes[2];

            return toreturn;
        }

        public int nextColor(int currcolor)
        {
            int toreturn = 0;

            byte r = (byte)(currcolor&0x110000 >> 16);
            byte g = (byte)(currcolor&0x001100 >> 8);
            byte b = (byte)(currcolor&0x000011);

            //one color is set, increment it. 
            if (r > 0 && r < 255 && b == 0 && g == 0)
                r++;
            if (g > 0 && g < 255 && r == 0 && g == 0)
                g++;
            if (b > 0 && b < 255 && r == 0 && g == 0)
                b++;



            return (r<<16)|(g<<8)|(b);
        }
    }
}

   

