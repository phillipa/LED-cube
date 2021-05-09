using System;
using System.Net.Sockets;
using System.Drawing;

namespace WonderWall
{
    public class NetworkHelper
    {

        private UdpClient udpClient;


        public NetworkHelper(string host, int port)
        {

            udpClient = new UdpClient(host, port);
        }

        /// <summary>
        /// Send an array of pixel colors to the UDP client
        /// </summary>
        /// <param name="pixels"></param>
        public void SendFrame(Color[] pixels)
        {
            Byte[] frame = GenerateFrame(pixels);

            int udp_packet_size = 500;//sending 500 byte packets

            Byte[] curr = new byte[udp_packet_size];//temp array to hold packet to send

            int idx = 0;//index into array to send

            while (idx < frame.Length)
            {
                int data_length = 0;
                for (int i = 0; i < udp_packet_size && idx < frame.Length; i++)
                {
                    curr[i] = frame[idx];
                    idx++;
                    data_length++;
                }
                udpClient.Send(curr, data_length);

            }

        }

        public void Close()
        {
            udpClient.Close();
        }

        /// <summary>
        /// Generate a byte array in the format expected by the teensy
        /// to send over the network.
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        private Byte[] GenerateFrame(Color[] pixels)
        {
            Byte[] END = { 0x01, 0x00, 0x01 };   // clear
            Byte[] START = { 0x01, 0x01, 0x00 }; // reset

            // Bytes = pixels + 2 for signaling * 3 for rgb
            Byte[] toreturn = new byte[(pixels.Length + 2) * 3];

            // Put reset to say new frame
            for (int i = 0; i < START.Length; i++)
                toreturn[i] = START[i];

            //pack trio of r/g/b for the color
            int pidx = 0; //index in pixel array
            int tridx = START.Length; //index into the byte array

            while(pidx < pixels.Length)
            {
                toreturn[tridx++] = pixels[pidx].R;
                toreturn[tridx++] = pixels[pidx].G;
                toreturn[tridx++] = pixels[pidx].B;
                pidx++;
            }

            // Put the clear to tell the thing to display frame.
            for (int i = END.Length; i > 0; i--)
                toreturn[toreturn.Length - i] = END[END.Length - i];

            return toreturn;
        }



    }
}
