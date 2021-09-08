﻿using System;
using freenect;
using System.Threading;
using WonderWall;
using System.Drawing;
using System.Diagnostics;

namespace KinectCalibrate
{
    class KinectFrameGrabber
    {
        Kinect kinect;
        BaseDataMap latestDepth, latestVideo;
        Thread t;
        public KinectFrameGrabber()
        {
            // Connect to first device
            kinect = new Kinect(0);
            kinect.Open();

            //Setup callbacks
            kinect.VideoCamera.DataReceived += this.HandleKinectVideoCameraDataReceived;
            kinect.DepthCamera.DataReceived += this.HandleKinectDepthCameraDataReceived;

            //Start the cameras
            kinect.VideoCamera.Start();
            kinect.DepthCamera.Start();

            t = new Thread(new ThreadStart(delegate ()
            {
                while (true)
                {
                    // Update status of accelerometer/motor etc.
                    kinect.UpdateStatus();

                    // Process any pending events.
                    Kinect.ProcessEvents();
                }
            }));
            t.Start();
        }

        public BaseDataMap GrabDepth()
        {
            return latestDepth;
        }

        public BaseDataMap GrabColor()
        {
            return latestVideo;
        }
        private void HandleKinectDepthCameraDataReceived(object sender, BaseCamera.DataReceivedEventArgs e)
        {
            latestDepth = e.Data;
        }
        private void HandleKinectVideoCameraDataReceived(object sender, BaseCamera.DataReceivedEventArgs e)
        {
            latestVideo = e.Data;
        }
    }


    class BackgroundSubtracter
    {

        UInt16[] background;

        public BackgroundSubtracter(UInt16[] bg)
        {
            background = bg;
        }

        public int AddBackground(UInt16[] new_bg)
        {
            if (new_bg.Length != background.Length)
            {
                Console.WriteLine("New background is the wrong size");
            }

            int updates = 0;
            for (int idx = 0; idx < background.Length; idx++)
            {
                background[idx] = (UInt16)(((double)background[idx] / 2 + (double)new_bg[idx] / 2));
            }
            return updates;
        }

        public UInt16[] SubtractBackground(UInt16[] image, UInt16 threshold = 0)
        {
            if (image.Length != background.Length)
            {
                Console.WriteLine("Image is the wrong size");
            }

            for (int idx = 0; idx < background.Length; idx++)
            {
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

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    int index = (width * y + x);

                    //Raw depth, quantized down from 11 bits to 8
                    int blue = (int)((double)data[index] * (double)Math.Pow(2, 8) / (double)Math.Pow(2, 11));

                    int red = 0;
                    int green = 0;

                    Color pxColor = Color.FromArgb(red, green, blue);
                    img.SetPixel(x, y, pxColor);
                }
            }
            img.Save(name);
        }

        static void Main(string[] args)
        {

            Console.WriteLine("There are {0} Kinect Devices Connected", Kinect.DeviceCount);
            if (Kinect.DeviceCount != 1)
            {
                Console.WriteLine("This program requires exactly one (1) Kinect to be connected.");
                return;
            }

            Console.WriteLine("Setting up network...");
            NetworkHelperWLED nh = new NetworkHelperWLED(UDP_HOST, UDP_PORT);
            Console.WriteLine("Setting up frame grabber...");
            KinectFrameGrabber kfg = new KinectFrameGrabber();

            Color[] pixels = new Color[FRAME_SIZE];
            for (int idx = 0; idx < pixels.Length; idx++)
            {
                pixels[idx] = Color.Black;
            }

            // int[][] corners = {
            //     new int[] {0, ROW_LEN*10, (ROW_LEN*10) +1},
            //     new int[] {ROW_LEN-1, ROW_LEN, ROW_LEN*3},
            //     new int[] {ROW_LEN*2, (ROW_LEN*2)+1, (ROW_LEN*12)-1 },
            //     new int[] {(ROW_LEN*3)-1, ROW_LEN*5, (ROW_LEN*5)-1},
            //     new int[] {(ROW_LEN*4)-1, (ROW_LEN*4)+1, (ROW_LEN*6)+1},
            //     new int[] {ROW_LEN*6, ROW_LEN*8, (ROW_LEN*8)+1},
            //     new int[] {ROW_LEN*7, (ROW_LEN*7)+1, (ROW_LEN*9)+1},
            //     new int[] {ROW_LEN*9, ROW_LEN*11, (ROW_LEN*11)+1}
            // };

            //Get a few frames of depth and create a background subtractor out of them
            Console.WriteLine("Waiting for camera data to be available...");
            while (kfg.GrabDepth() == null)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Calculating background subtraction...");
            BackgroundSubtracter bg_sub = new BackgroundSubtracter(convertDepthToUInts(kfg.GrabDepth().Data));
            for (int ii = 0; ii < 10; ii++)
            {
                int change_count = bg_sub.AddBackground(convertDepthToUInts(kfg.GrabDepth().Data));
                Thread.Sleep(100);
            }

            Console.WriteLine("Starting main loop...");
            Stopwatch frame_sw = new Stopwatch();
            while (true)
            {
                frame_sw.Start();
                UInt16[] curr_depth = convertDepthToUInts(kfg.GrabDepth().Data);
                curr_depth = bg_sub.SubtractBackground(curr_depth);

                long average = 0;
                for (int ii = 0; ii < curr_depth.Length; ii++)
                {
                    average += curr_depth[ii];
                }
                average = average / curr_depth.Length;

                Console.WriteLine("Average depth value with background removed: {0}", average);

                Random random = new Random();
                for (int ii = 0; ii < pixels.Length; ii++)
                {
                    //Saw values in the range 10-400ish when I was testing
                    if (random.Next(10, 200) < average)
                    {
                        pixels[ii] = Color.Magenta;
                    }
                    else
                    {
                        pixels[ii] = Color.Black;
                    }
                }
                nh.Send(pixels);

                frame_sw.Stop();
                //Cast from long to int is fine because the max is like 42. 
                //42 is because 24 fps = 24/1000 = 41.66667ms/frame
                //Thread.Sleep((int)Math.Max(0, 42 - frame_sw.ElapsedMilliseconds));
                Thread.Sleep((int)Math.Max(0, 100 - frame_sw.ElapsedMilliseconds));
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
            UInt16[] res = new UInt16[(int)Math.Floor((double)(depthData.Length / 2))];
            int res_idx = 0;
            //convert depth bytes to UINT16 here. 
            for (int i = 0; i < depthData.Length; i += 2)
            {
                UInt16 curr = (UInt16)(depthData[i + 1] & 0x7); //get lower 3 bits of first byte
                curr <<= 8;//shift the byte over for the next byte
                curr |= depthData[i];
                if (res_idx < res.Length)
                    res[res_idx] = curr;
                else
                    Console.WriteLine("this should not happen");
                res_idx++;

            }

            return res;
        }
    }
}
