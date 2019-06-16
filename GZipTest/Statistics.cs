using System;
using System.Diagnostics;

namespace GZipTest
{
    public class Statistics
    {
        public readonly int timeoutMilliseconds = 100;
        public readonly long blockSizeBytes = 1048576;
        public int workerThreads = Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 1 : Environment.ProcessorCount;
        public long totalBytesRead = 0;
        public long totalBytesWritten = 0;
        public long inputWaitMilliseconds = 0;
        public long outputWaitMilliseconds = 0;
        public long compressionTimeMilliseconds = 0;
        
        public Stopwatch totalTime = Stopwatch.StartNew();
        public Stopwatch diskReadTime = new Stopwatch();
        public Stopwatch diskWriteTime = new Stopwatch();

        private const int intermediateAfterMilliseconds = 2000;
        private bool intermediateAlreadyShown = false;

        private double ToMBps(double x) => x / 1048.576;
        private double Throughput => ToMBps (Math.Max(totalBytesRead, totalBytesWritten) / totalTime.ElapsedMilliseconds);
        private double DiskReadSpeed => ToMBps (totalBytesRead / diskReadTime.ElapsedMilliseconds);
        private double DiskWriteSpeed => ToMBps (totalBytesWritten / diskWriteTime.ElapsedMilliseconds);
        private double CompressSpeed => ToMBps (Math.Max(totalBytesRead, totalBytesWritten) / compressionTimeMilliseconds);
        private double InputWaitPercent => (double) inputWaitMilliseconds / (inputWaitMilliseconds + compressionTimeMilliseconds + outputWaitMilliseconds);
        private double OutputWaitPercent => (double) outputWaitMilliseconds / (inputWaitMilliseconds + compressionTimeMilliseconds + outputWaitMilliseconds);
        private long PeakMemoryMB => Process.GetCurrentProcess().PeakWorkingSet64 / 1048576;
        private double ReadUtilization => (double) diskReadTime.ElapsedMilliseconds / totalTime.ElapsedMilliseconds;
        private double WriteUtilization => (double) diskWriteTime.ElapsedMilliseconds / totalTime.ElapsedMilliseconds;
        private double CompressionUtilization => (double) compressionTimeMilliseconds / workerThreads / totalTime.ElapsedMilliseconds;

        public void WriteStartMessages()
        {
            Console.WriteLine($"Block size set to {blockSizeBytes} bytes.");
            Console.WriteLine($"Starting 1 reading thread and {workerThreads} compression threads.");
        }

        public void WriteIntrermediateStatistics()
        {
            if (intermediateAlreadyShown || totalTime.ElapsedMilliseconds < intermediateAfterMilliseconds) return;
            intermediateAlreadyShown = true;
            Console.WriteLine("Intermediate statistics");
            Console.WriteLine($"Peak working memory: {PeakMemoryMB:F0} MB");
            Console.WriteLine($"Disk reading speed: {DiskReadSpeed:F0} MB/s, Reading thread utilization: {ReadUtilization:P3}");
            Console.WriteLine($"Disk writing speed: {DiskWriteSpeed:F0} MB/s, Writing thread utilization: {WriteUtilization:P3}");
            Console.WriteLine($"Application run time: {(double)totalTime.ElapsedMilliseconds / 1000 :F3} s");
            Console.WriteLine($"Throughput: {Throughput:F3} MB/s");
            Console.WriteLine($"Processing continues...\r\n");
        }

        public void WriteEndStatistics()
        {
            Console.WriteLine("Final statistics");
            Console.WriteLine($"Peak working memory: {PeakMemoryMB:F0} MB");
            Console.WriteLine($"Disk reading speed: {DiskReadSpeed:F0} MB/s, Reading thread utilization: {ReadUtilization:P3}");
            Console.WriteLine($"Disk writing speed: {DiskWriteSpeed:F0} MB/s, Writing thread utilization: {WriteUtilization:P3}");
            Console.WriteLine($"Compresss speed: {CompressSpeed:F0} MB/s each thread, Compression threads utilization: {CompressionUtilization:P3}");
            Console.WriteLine($"Time wasted waiting for input queues: {InputWaitPercent:P3}");
            Console.WriteLine($"Time wasted waiting for output dictionary: {OutputWaitPercent:P3}");
            Console.WriteLine($"Application run time: {(double)totalTime.ElapsedMilliseconds / 1000:F3} s");
            Console.WriteLine($"Total throughput: {Throughput:F3} MB/s");
        }
    }
}
