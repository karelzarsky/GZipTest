using System.Diagnostics;

namespace GZipTest
{
    public interface IStatistics
    {
        long BlockSizeBytes { get; }
        long CompressionTimeMilliseconds { get; set; }
        Stopwatch DiskReadTime { get; set; }
        Stopwatch DiskWriteTime { get; set; }
        long InputWaitMilliseconds { get; set; }
        long OutputWaitMilliseconds { get; set; }
        int TimeoutMilliseconds { get; }
        long TotalBytesRead { get; set; }
        long TotalBytesWritten { get; set; }
        Stopwatch TotalTime { get; set; }
        int WorkerThreads { get; set; }

        void WriteEndStatistics();
        void WriteIntrermediateStatistics();
        void WriteStartMessages();
    }
}