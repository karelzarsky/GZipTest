using System.Diagnostics;

namespace GZipTest
{
    public interface IStatistics
    {
        long CompressionTimeMilliseconds { get; set; }
        long InputWaitMilliseconds { get; set; }
        long OutputWaitMilliseconds { get; set; }
        long TotalBytesRead { get; set; }
        long TotalBytesWritten { get; set; }
        Stopwatch TotalTime { get; }
        Stopwatch DiskReadTime { get; }
        Stopwatch DiskWriteTime { get; }

        void WriteEarlyStatistics();
        void WriteEndStatistics();
        void WriteStartMessages();
    }
}