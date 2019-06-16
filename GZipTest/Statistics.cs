using System;
using System.Diagnostics;

namespace GZipTest
{
    public class Statistics
    {
        const double MEGABYTES_PER_Millisecond = 1048.576;

        public readonly int timeoutMillisecond = 100;
        public readonly long blockSize = 1048576;
        public int compressionThreads = 0;
        public long totalBytesRead = 0;
        public long totalBytesWritten = 0;
        public long inputWaitMillisecond = 0;
        public long outputWaitMillisecond = 0;
        public long compressionTimeMillisecond = 0;
        public long totalBlocks;
        
        public Stopwatch totalTime = Stopwatch.StartNew();
        public Stopwatch diskReadTime = new Stopwatch();
        public Stopwatch diskWriteTime = new Stopwatch();

        private double Throughput => Math.Max(totalBytesRead, totalBytesWritten) / totalTime.ElapsedMilliseconds / MEGABYTES_PER_Millisecond;
        private double DiskReadSpeed => totalBytesRead / diskReadTime.ElapsedMilliseconds / MEGABYTES_PER_Millisecond;
        private double DiskWriteSpeed => totalBytesWritten / diskWriteTime.ElapsedMilliseconds / MEGABYTES_PER_Millisecond;
        private double compressSpeed => Math.Max(totalBytesRead, totalBytesWritten) / compressionTimeMillisecond / MEGABYTES_PER_Millisecond;
        private double inputWaitPercent => (double) inputWaitMillisecond / (inputWaitMillisecond + compressionTimeMillisecond + outputWaitMillisecond);
        private double outputWaitPercent => (double) outputWaitMillisecond / (inputWaitMillisecond + compressionTimeMillisecond + outputWaitMillisecond);
        private long peakMemory => Process.GetCurrentProcess().PeakWorkingSet64 / 1048576;
        public void WriteStartMessages()
        {
            Console.WriteLine($"Block size set to {blockSize} bytes.");
            Console.WriteLine($"Starting {compressionThreads} compression threads.");
        }

        public void WriteEndStatistics()
        {
            Console.WriteLine($"Peak working memory: {peakMemory:F0} MB.");
            Console.WriteLine($"Disk reading speed {DiskReadSpeed:F0} MB/s.");
            Console.WriteLine($"Disk writing speed {DiskWriteSpeed:F0} MB/s.");
            Console.WriteLine($"Compresss speed {compressSpeed:F0} MB/s each thread.");
            Console.WriteLine($"Input queues processing and wait time {inputWaitPercent:P3}.");
            Console.WriteLine($"Output dictionary processing and wait time {outputWaitPercent:P3}");
            Console.WriteLine($"Total time: {totalTime.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Total throughput: {Throughput:F3} MB/s.");
        }
    }
}
