using System;
using freenect;
using System.Threading;
using WonderWall;
using System.Drawing;

namespace KinectCalibrate
{
    class KinectFrameGrabber
    {
        Kinect kinect;
        BaseDataMap latestDepth, latestVideo;
        Thread t;
        public KinectFrameGrabber(){
            // Connect to first device
            kinect = new Kinect(0);
            kinect.Open();

            //Setup callbacks
            kinect.VideoCamera.DataReceived += this.HandleKinectVideoCameraDataReceived;
            kinect.DepthCamera.DataReceived += this.HandleKinectDepthCameraDataReceived;            
        
            //Start the cameras
            kinect.VideoCamera.Start();
            kinect.DepthCamera.Start();

            t = new Thread(new ThreadStart(delegate()
            {
                while(true)
                {
                    //Console.Write(".");
                    // Update status of accelerometer/motor etc.
                    kinect.UpdateStatus();

                    // Process any pending events.
                    Kinect.ProcessEvents();
                    //Console.Write("-");
                }
            }));
            t.Start();
        }

        public BaseDataMap GrabDepth(){
            //Console.WriteLine("GrabDepth called");
            return latestDepth;
        }

        public BaseDataMap GrabColor(){
            //Console.WriteLine("GrabColor called");
            return latestVideo;
        }
        private void HandleKinectDepthCameraDataReceived (object sender, BaseCamera.DataReceivedEventArgs e)
		{
			//Console.WriteLine("Depth data received at {0} is {1} @ {2}FPS", e.Timestamp, e.Data.CaptureMode, e.Data.CaptureMode.FrameRate);
			latestDepth = e.Data;
		}
        private void HandleKinectVideoCameraDataReceived (object sender, BaseCamera.DataReceivedEventArgs e)
		{
			//Console.WriteLine("Video data received at {0} is {1} @ {2}FPS", e.Timestamp, e.Data.CaptureMode, e.Data.CaptureMode.FrameRate);
            latestVideo = e.Data;
		}
    }
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

            Console.WriteLine("There are {0} Kinect Devices Connected", Kinect.DeviceCount);
			if(Kinect.DeviceCount != 1)
            {
                Console.WriteLine("This program requires exactly one (1) Kinect to be connected.");
                return;    
            }

            NetworkHelperWLED nh = new NetworkHelperWLED(UDP_HOST, UDP_PORT);
            KinectFrameGrabber kfg = new KinectFrameGrabber();

            Color[] pixels = new Color[FRAME_SIZE];
            for(int idx = 0; idx < pixels.Length; idx++)
            {
                pixels[idx] = Color.Black;
            }
            
            int[][] corners = {
                new int[] {0, ROW_LEN*10, (ROW_LEN*10) +1},
                new int[] {ROW_LEN-1, ROW_LEN, ROW_LEN*3},
                new int[] {ROW_LEN*2, (ROW_LEN*2)+1, (ROW_LEN*12)-1 },
                new int[] {(ROW_LEN*3)-1, ROW_LEN*5, (ROW_LEN*5)-1},
                new int[] {(ROW_LEN*4)-1, (ROW_LEN*4)+1, (ROW_LEN*6)+1},
                new int[] {ROW_LEN*6, ROW_LEN*8, (ROW_LEN*8)+1},
                new int[] {ROW_LEN*7, (ROW_LEN*7)+1, (ROW_LEN*9)+1},
                new int[] {ROW_LEN*9, ROW_LEN*11, (ROW_LEN*11)+1}
            };

            BaseDataMap last_depth, curr_depth;
            last_depth = kfg.GrabDepth();
            while(true)
            {
                curr_depth = kfg.GrabDepth();
                //Thread.Sleep(250);
                //depth2 = kfg.GrabDepth();
        
                if((last_depth != null) && (curr_depth != null))
                {
                    //Basic byte diff
                    int count_diff = 0;
                    int p_idx = 0;
                    for(int i = 0; i < curr_depth.CaptureMode.Size; i+=3){
                      
                      int argb = curr_depth.Data[i];
                        argb |= curr_depth.Data[i+1]<<8;
                        argb |= curr_depth.Data[i+2]<<16;
                        if(p_idx < pixels.Length){
                            if(pixels[p_idx] != Color.FromArgb(argb))
                                count_diff++;
                            pixels[p_idx] = Color.FromArgb((int)argb);
                        }
                        p_idx++;
                        
                    }
                    if(count_diff > 0)
                        Console.WriteLine("{0} of {1} changed", count_diff, curr_depth.CaptureMode.Size);
                    nh.Send(pixels);
                }

                last_depth = curr_depth;
            }
            // while(true){    
            //     foreach(int[] corner in corners){
            //         BaseDataMap depthData = kfg.GrabDepth();

            //         foreach(int address in corner){
            //             pixels[address] = Color.Red;
            //         }
            //         nh.Send(pixels);
            //         Thread.Sleep(250);
            //         BaseDataMap redOnData = kfg.GrabColor();

            //         foreach(int address in corner){
            //             pixels[address] = Color.Black;
            //         }
            //         nh.Send(pixels);
            //         Thread.Sleep(250);
            //         BaseDataMap redOffData = kfg.GrabColor();
            //     }
            // }            
        }
    }
}
