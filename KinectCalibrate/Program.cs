using System;
using freenect;
using System.Threading;
using WonderWall;
using System.Drawing;

namespace KinectCalibrate
{
    class Program
    {
        //TODO how does wled do auto-discovery?
        //Network params
        public const string UDP_HOST = "192.168.2.127";
        public const int UDP_PORT = 21324;
        public const int ROW_LEN = 91; //length of tube/row
        public const int FRAME_SIZE = ROW_LEN * 3 * 4;

        static void Main(string[] args)
        {
            NetworkHelperWLED nh = new NetworkHelperWLED(UDP_HOST, UDP_PORT);
            Color[] pixels = new Color[FRAME_SIZE];
            for(int idx = 0; idx < pixels.Length; idx++)
            {
                pixels[idx] = Color.Black;
            }
            
            int[][] corners = {
                new int[] {0, ROW_LEN*10, (ROW_LEN*10) +1},
            };

            
            pixels[0] = Color.Red;
            pixels[(ROW_LEN*10)] = Color.Red;
            pixels[(ROW_LEN*10)+1] = Color.Red;

            pixels[ROW_LEN-1] = Color.Green;
            pixels[ROW_LEN] = Color.Green;
            pixels[ROW_LEN*3] = Color.Green;

            pixels[ROW_LEN*2] = Color.Blue;
            pixels[ROW_LEN*2+1] = Color.Blue;
            pixels[(ROW_LEN*12)-1] = Color.Blue;

            pixels[ROW_LEN*3-1] = Color.Purple;
            pixels[(ROW_LEN*5)] = Color.Purple;
            pixels[(ROW_LEN*5)-1] = Color.Purple;
            
            pixels[(ROW_LEN*4)-1] = Color.Orange;
            pixels[(ROW_LEN*4)+1] = Color.Orange;
            pixels[(ROW_LEN*6)+1] = Color.Orange;
            
            pixels[ROW_LEN*6] = Color.White;
            pixels[ROW_LEN*8] = Color.White;
            pixels[(ROW_LEN*8)+1] = Color.White;
            
            pixels[ROW_LEN*7] = Color.Chartreuse;
            pixels[(ROW_LEN*7)+1] = Color.Chartreuse;
            pixels[(ROW_LEN*9)+1] = Color.Chartreuse;
    
            pixels[(ROW_LEN*9)] = Color.Magenta;
            pixels[(ROW_LEN*11)] = Color.Magenta;
            pixels[(ROW_LEN*11)+1] = Color.Magenta;
        
            while(true){
            nh.Send(pixels);
            Thread.Sleep(500);
            }
            //Get the kinect color image
            //Find the reddest point
            //Find the position of that point in the depth image
            //Repeat for all four corners
            
        }
    }
}
