using System.Diagnostics;

namespace GZipTest
{
    public interface IStatistics
    {
        long CompressionTimeMilliseconds { get; set; }
        Stopwatch DiskReadTime { get; set; }
        Stopwatch DiskWriteTime { get; set; }
        long InputWaitMilliseconds { get; set; }
        long OutputWaitMilliseconds { get; set; }
        long TotalBytesRead { get; set; }
        long TotalBytesWritten { get; set; }
        Stopwatch TotalTime { get; set; }

        void WriteEarlyStatistics();
        void WriteEndStatistics();
        void WriteStartMessages();
    }
}