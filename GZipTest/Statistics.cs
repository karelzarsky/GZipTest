using System;
using System.Diagnostics;

namespace GZipTest
{
    public class Statistics : IStatistics
    {
        private const int megabyte = 1048576;
        private const int statsDelayMilliseconds = 2000;
        private bool statsAlreadyShown = false;
        private double ConvertToMBps(double x) => x / 1048.576;
        private double Throughput => TotalTime.ElapsedMilliseconds == 0 ? 0 : ConvertToMBps(Math.Max(TotalBytesRead, TotalBytesWritten) / TotalTime.ElapsedMilliseconds);
        private double DiskReadSpeed => DiskReadTime.ElapsedMilliseconds == 0 ? 0 : ConvertToMBps(TotalBytesRead / DiskReadTime.ElapsedMilliseconds);
        private double DiskWriteSpeed => DiskWriteTime.ElapsedMilliseconds == 0 ? 0 : ConvertToMBps(TotalBytesWritten / DiskWriteTime.ElapsedMilliseconds);
        private double CompressSpeed => CompressionTimeMilliseconds == 0 ? 0 : ConvertToMBps(Math.Max(TotalBytesRead, TotalBytesWritten) / CompressionTimeMilliseconds);
        private double InputWaitPercent => (double)InputWaitMilliseconds / (InputWaitMilliseconds + CompressionTimeMilliseconds + OutputWaitMilliseconds);
        private double OutputWaitPercent => (double)OutputWaitMilliseconds / (InputWaitMilliseconds + CompressionTimeMilliseconds + OutputWaitMilliseconds);
        private double ReadUtilization => TotalTime.ElapsedMilliseconds == 0 ? 1 : (double)DiskReadTime.ElapsedMilliseconds / TotalTime.ElapsedMilliseconds;
        private double WriteUtilization => TotalTime.ElapsedMilliseconds == 0 ? 1 : (double)DiskWriteTime.ElapsedMilliseconds / TotalTime.ElapsedMilliseconds;
        private double WorkerUtilization => TotalTime.ElapsedMilliseconds == 0 ? 1 : (double)CompressionTimeMilliseconds / Settings.WorkerThreads / TotalTime.ElapsedMilliseconds;
        private double CompressionRatio => TotalBytesRead == 0 ? 0 : (double)TotalBytesWritten / TotalBytesRead;
        private long PeakMemoryMB => Process.GetCurrentProcess().PeakWorkingSet64 / megabyte;
        private readonly ISettings Settings;

        public long TotalBytesRead { get; set; } = 0;
        public long TotalBytesWritten { get; set; } = 0;
        public long InputWaitMilliseconds { get; set; } = 0;
        public long OutputWaitMilliseconds { get; set; } = 0;
        public long CompressionTimeMilliseconds { get; set; } = 0;
        public Stopwatch TotalTime { get; set; } = Stopwatch.StartNew();
        public Stopwatch DiskReadTime { get; set; } = new Stopwatch();
        public Stopwatch DiskWriteTime { get; set; } = new Stopwatch();

        public Statistics(ISettings settings)
        {
            Settings = settings;
        }

        public void WriteStartMessages()
        {
            Console.WriteLine($"Block size: {Settings.BlockSizeBytes} bytes");
            Console.WriteLine($"Starting 1 reading thread and {Settings.WorkerThreads} worker threads.\r\n");
        }

        public void WriteEarlyStatistics()
        {
            if (statsAlreadyShown || TotalTime.ElapsedMilliseconds < statsDelayMilliseconds)
                return;

            statsAlreadyShown = true;
            Console.WriteLine("Preliminary statistics");
            Console.WriteLine("----------------------");
            Console.WriteLine($"Peak working memory: {PeakMemoryMB:F0} MB");
            Console.WriteLine($"Throughput: {Throughput:F3} MB/s");
            Console.WriteLine($"Average disk reading speed: {DiskReadSpeed:F0} MB/s, Reading thread utilization: {ReadUtilization:P3}");
            Console.WriteLine($"Average disk writing speed: {DiskWriteSpeed:F0} MB/s, Writing thread utilization: {WriteUtilization:P3}");
            Console.WriteLine("Processing continues, please wait...\r\n");
        }

        public void WriteEndStatistics()
        {
            Console.WriteLine("Final statistics");
            Console.WriteLine("================");
            Console.WriteLine($"Peak working memory: {PeakMemoryMB:F0} MB");
            Console.WriteLine($"Average disk reading speed: {DiskReadSpeed:F0} MB/s, Reading thread utilization: {ReadUtilization:P3}");
            Console.WriteLine($"Average disk writing speed: {DiskWriteSpeed:F0} MB/s, Writing thread utilization: {WriteUtilization:P3}");
            Console.WriteLine($"Average compresss speed: {CompressSpeed:F0} MB/s each thread, Workers utilization: {WorkerUtilization:P3}");
            Console.WriteLine($"Time spent waiting for input queues: {InputWaitPercent:P3}");
            Console.WriteLine($"Time spent waiting for output buffer: {OutputWaitPercent:P3}");
            Console.WriteLine($"Compression ratio: {CompressionRatio:P3}");
            Console.WriteLine($"Application run time: {(double)TotalTime.ElapsedMilliseconds / 1000:F3} s");
            Console.WriteLine($"Total throughput: {Throughput:F3} MB/s");
        }
    }
}
