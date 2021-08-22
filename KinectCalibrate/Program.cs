using System;
using freenect;
using System.Threading;
using WonderWall;
using System.Drawing;
using System.Collections.Generic;


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


    class BackgroundSubtracter{

        UInt16[] background;

        public BackgroundSubtracter(UInt16[] bg)
        {
            background = bg;
        }

        public int AddBackground(UInt16[] new_bg)
        {
            if(new_bg.Length != background.Length)
            {
                Console.WriteLine("New background is the wrong size");
            }

            int updates = 0;
            for(int idx = 0; idx < background.Length; idx++)
            {
                Kinect data is 0x07 in high byte if there was no return, so replace no-return with updated value
                if((background[idx] >> 8 == 0x07) && (new_bg[idx] >> 8 == 0x03))
                {
                    background[idx] = new_bg[idx];
                    updates+=1;
                }
                Otherwise, get the (integer) average
                else if ((background[idx] >> 8 == 0x07) && (new_bg[idx] >> 8 == 0x07)){
                    background[idx] = (UInt16)(((double)background[idx]/2 + (double)new_bg[idx]/2));
                }
            }
            return updates;
        }

        public UInt16[] SubtractBackground(UInt16[] image, UInt16 threshold = 0)
        {
            if(image.Length != background.Length)
            {
                Console.WriteLine("Image is the wrong size");
            }

            for(int idx = 0; idx < background.Length; idx++)
            {
                //Don't use no-return values
                if(image[idx] >> 8 == 0x03)
                {
                    image[idx] = 0;
                    continue;
                }
                image[idx] = (UInt16)Math.Max(0, image[idx] - (background[idx] + threshold));
            }
            //TODO is this a copy or reference semantics language?
            return image;
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

        static void DumpImage(UInt16[] data, String name)
        {

            //Assumes data is a 640x480 image (depth data from the Kinect)
            int width = 640;
            int height = 480;
            Bitmap img = new Bitmap(width, height);
            
            int x, y;
            
            for(x = 0; x < width; x++)
            {        
                for(y = 0; y < height; y++)
                {
                    int index = (width*y + x);
                    
                    //Raw depth, quantized down from 11 bits to 8
                    int blue = (int)((double) data[index] * (double)Math.Pow(2,8)/(double)Math.Pow(2, 11));
                    
                    int red = 0;
                    int green = 0;
                    // For detecting no-return areas
                    // if(data[index] >> 8 == 0x07)
                    // {
                    //     red = 255;
                    // }
                    // if(data[index] >> 8 == 0x03)
                    // {
                    //     green = 255;
                    // }
                    Color pxColor = Color.FromArgb(red, green, blue);
                    img.SetPixel(x, y, pxColor);
                }
            }
            img.Save(name);
        }

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


            //Get a few frames of depth and create a background subtractor out of them
            while(kfg.GrabDepth() == null){}
            BackgroundSubtracter bg_sub = new BackgroundSubtracter(convertDepthToUInts(kfg.GrabDepth().Data));
            for(int ii = 0; ii < 10; ii++)
            {
                int change_count = bg_sub.AddBackground(convertDepthToUInts(kfg.GrabDepth().Data));
                Console.WriteLine("Update {0} changed {1} px", ii, change_count);
                Thread.Sleep(100);
            }

            //Get a new depth image
            UInt16[] depth_img = convertDepthToUInts(kfg.GrabDepth().Data);
            DumpImage(depth_img, "depth_img.png");
            UInt16[] no_bg = bg_sub.SubtractBackground(depth_img);
            DumpImage(no_bg, "bg_subtracted.png");

            //System.IO.File.WriteAllBytes("raw_depth.bin", kfg.GrabDepth().Data);

            //DumpImage(convertDepthToUInts(kfg.GrabDepth().Data), "prefix_test.png"); 
            System.Environment.Exit(0);

            BaseDataMap last_depth, curr_depth;
            UInt16[] last_depth_uint, curr_depth_uint;
            last_depth = kfg.GrabDepth();
            last_depth_uint = convertDepthToUInts(last_depth.Data);
            curr_depth_uint = null;
            Color[] p_help = new Color[640*480*2];
            int num_samples = 0;
            int count_diff = 0;
            
            while(true)
            {
                curr_depth = kfg.GrabDepth();       
                if(curr_depth != null)
                    curr_depth_uint = convertDepthToUInts(curr_depth.Data);
                //Thread.Sleep(250);
                //depth2 = kfg.GrabDepth();
        
                if((last_depth != null) && (curr_depth != null))
                {

                    int num_diff = 0;
                    for(int i = 0; i< curr_depth_uint.Length;i++)
                    {
                        if(curr_depth_uint[i]!=last_depth_uint[i])
                            num_diff++;
                    }

                    Console.WriteLine("Num different "+num_diff+ " out of "+last_depth_uint.Length);
                    //Basic byte diff
                    
                    /*int p_idx = 0;
                    for(int i = 0; i < curr_depth.CaptureMode.Size; i+=3){
                      
                      int argb = curr_depth.Data[i];
                        argb |= curr_depth.Data[i+1]<<8;
                        argb |= curr_depth.Data[i+2]<<16;
                        if(p_idx < p_help.Length){
                            if(p_help[p_idx] != Color.FromArgb(argb))
                            {    count_diff++;
                                    p_help[p_idx] = Color.FromArgb((int)argb);
                            }
                        }
                        p_idx++;
                        
                    }
                   // if(count_diff > 0)
                      //  Console.WriteLine("{0} of {1} changed", count_diff, curr_depth.CaptureMode.Size);
                        num_samples++;
                        if(num_samples == 24)
                        {
                            Console.WriteLine("Average of {0} of {1} changes in 1s ", (count_diff/num_samples), curr_depth.CaptureMode.Size);
                       
                            double avg_perc = 100*((double)(count_diff/num_samples)/(double)(curr_depth.CaptureMode.Size/3));
                            
                            int pixels_per_side = (int)Math.Round((double)(91*(avg_perc/100)));
                            Console.WriteLine("avg perc "+avg_perc+" pixels per side "+ pixels_per_side);
                            for(int p = 0; p<pixels.Length;p++)
                                pixels[p] = Color.Black;
                            for(int p = 0; p<pixels.Length;p+=91)
                            {
                                for(int side_pix=0; side_pix<pixels_per_side; side_pix++)
                                {
                                    if((p+side_pix)<pixels.Length)
                                        pixels[p+side_pix] = Color.Aqua;
                                }
                            }
                            count_diff=0;
                            num_samples=0;
                        }
                    */
                    nh.Send(pixels);
                }

                last_depth = curr_depth;
                last_depth_uint = curr_depth_uint;
                Thread.Sleep(1000/24); //24 fps
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

        static UInt16[] convertDepthToUInts(byte[] depthData)
        {
            UInt16[] res = new UInt16[(int)Math.Floor((double)(depthData.Length/2))];
            int res_idx = 0;
            //convert depth bytes to UINT16 here. 
            for(int i = 0; i<depthData.Length; i+=2)
            {
                UInt16 curr = (UInt16)(depthData[i+1] & 0x7); //get lower 3 bits of first byte
                curr <<= 8;//shift the byte over for the next byte
                curr |= depthData[i];
                if(res_idx<res.Length)
                    res[res_idx] = curr;
                else
                    Console.WriteLine("this should not happen");
                res_idx++;

            }

            return res;
        }
    }
}
