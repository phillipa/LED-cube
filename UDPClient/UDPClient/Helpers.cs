using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDUDPClient
{
    /// <summary>
    /// Helper methods for diagnostics, etc. 
    /// </summary>
    public class Helpers
    {
        public static void WriteLEDBenchmarks(int frameSize, int sequenceLength, int iterations, int delay, TimeSpan elapsedTime)
        {
            long frameWrites = sequenceLength * iterations;
            long totalBytes = frameWrites * 3 * (frameSize + 2);
            long writeTime = (long)elapsedTime.Milliseconds - sequenceLength * iterations * delay;
            double frameRate = frameWrites * 1000 / elapsedTime.Milliseconds;
            double pixelRate = frameRate * frameSize;
            double byteRate = frameRate * 3 * (frameSize + 2);
            double throughputPixels = pixelRate / 30;


            Console.WriteLine("========LED BENCHMARKS========");
            Console.WriteLine($"Total number of frame writes: {frameWrites}");
            Console.WriteLine($"Total number of iterations: {iterations}");
            Console.WriteLine($"Frame size: {frameSize} pixels");
            Console.WriteLine($"Bytes per frame: {3 * (frameSize + 2)} bytes");
            Console.WriteLine($"Total bytes written: {totalBytes} bytes");
            Console.WriteLine($"Pixel delay: {delay} ms");
            Console.WriteLine($"Elapsed time: {elapsedTime.Milliseconds} ms ");
            Console.WriteLine($"Estimated write time {writeTime} ms");
            Console.WriteLine($"Frame rate: {frameRate} frames/s");
            Console.WriteLine($"Pixel rate: {pixelRate} pixels/s");
            Console.WriteLine($"Byte rate: {byteRate} bytes/s");
            Console.WriteLine($"Estimated throughput: {throughputPixels} LEDs " +
                $"at 30 frames/s");
        }

    }
}
