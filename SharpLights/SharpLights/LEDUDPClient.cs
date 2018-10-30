using System;
using System.Net.Sockets;

namespace SharpLights
{
    public class LEDUDPClient
    {
        public static readonly Byte[] END = { 0x01, 0x00, 0x01 };   // clear
        public static readonly Byte[] START = { 0x01, 0x01, 0x00 }; // reset

        public string host = "192.168.0.18";
        public int port = 7777;
        public int framesize = 27 * 8;

        UdpClient udpClient;

        public LEDUDPClient(string p_host, int p_port, int p_framesize)
        {
            this.host = p_host;
            this.port = p_port;
            this.framesize = p_framesize;
            this.udpClient = new UdpClient(host, port);
        }

        /// <summary>
        /// Send a frame of bytes to the netcat server.
        /// This function adds the RST/CLR flags for the teensy
        /// to know frames are starting/ending 
        /// </summary>
        /// <param name="pixels">Pixels.</param>
        public void sendFrame(byte[] pixels)
        {
            //TODO: output warning if pixels isn't length of frame
            //TODO: check for min/max UDP packet lengths
            Byte[] frame = new byte[pixels.Length + 6]; //pixels + RST/CLR

            //put reset to say new frame
            for (int i = 0; i < START.Length; i++)
                frame[i] = START[i];

            for (int i = 0; i < pixels.Length; i++)
                frame[3 + i] = pixels[i];

            for (int i = 0; i < END.Length; i++)
                frame[3 + pixels.Length + i] = END[i];

            udpClient.Send(frame, frame.Length);

        }

        public void sendFrame(Color[] pixels)
        {
            Byte[] frame = new byte[pixels.Length * 3 + 6];
            //put reset to say new frame
            for (int i = 0; i < START.Length; i++)
                frame[i] = START[i];

            for (int i = 0; i < pixels.Length; i++)
            {
                frame[3 + i * 3] = pixels[i].red;
                frame[3 + i * 3 + 1] = pixels[i].green;
                frame[3 + i * 3 + 2] = pixels[i].blue;

            }
            for (int i = 0; i < END.Length; i++)
                frame[3 + pixels.Length + i] = END[i];

            udpClient.Send(frame, frame.Length);

        }

        public void closeSocket()
        {
            udpClient.Close();
        }

    }
}
