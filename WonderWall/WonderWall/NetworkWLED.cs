using System;
using System.Net.Sockets;
using System.Drawing;

namespace WonderWall
{
    /* This is an implementation of the DNRGB protocol for communicating with WLED, 
     * which in turn tells LED strips what to do. The default port for WLED is 21324. 
     *
     * Per packet, the protocol is as follows (based on https://github.com/Aircoookie/WLED/wiki/UDP-Realtime-Control)
     * Byte 0 - protocol, 4 is DNRGB
     * Byte 1 - delay for return to normal mode, 255 is lock in UDP mode until changed
     * Byte 2 - start index high byte
     * Byte 3 - start index low byte
     * Byte 4 + n*3 - Red value for LED n
     * Byte 5 + n*3 - Green value for LED n
     * Byte 6 + n*3 - Blue value for LED n
     * 
     * Note that this does mean that an individual LED can be changed with a single packet, 
     * and that changes can be applied as deltas, rather than updating the whole array. 
     */
    public class NetworkHelperWLED
    {
        private UdpClient udpClient;
     
        public NetworkHelper(string host, int port)
        {
            udpClient = new UdpClient(host, port);
        }

        public void Send(Color[] pixels, UInt16 offset = 0)
        {
            //DNRGB mode supports up to 489 LEDs per packet
            int max_leds = 489;
            
            //Track how many of the pixels have been sent
            int sent = 0;
            while(sent < pixels.Length){
                //DNRGB header for this packet
                Byte[] header = {0x04, 0x01, (byte) ((offset + sent) >> 8), (byte) ((offset + sent) & 0xff)};
            
                // Calculate how many of the pixels are going in this packet
                int to_send = min(pixels.Length-sent, max_leds);

                // Packet data length is header + pixel data
                Byte[] packet_data = new byte [4 + (to_send * 3)];
                header.CopyTo(packet_data, 0);
                // Copy the pixel data in
                int pix_idx = sent;
                int data_idx = 4;
                while(pix_idx < sent+to_send){
                    packet_data[data_idx++] = pixels[pix_idx].R;
                    packet_data[data_idx++] = pixels[pix_idx].G;
                    packet_data[data_idx++] = pixels[pix_idx].B;
                    pix_idx++;
                }

                udpClient.Send(packet_data, packet_data.Length);
                sent += to_send;
            }
        }

        public void Close()
        {
            udpClient.Close();
        }
    }
}
