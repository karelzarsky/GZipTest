using System.IO.Compression;

namespace GZipTest
{
    public interface ISettings
    {
        long BlockSizeBytes { get; }
        CompressionMode Mode { get; set; }
        int MonitorTimeoutMilliseconds { get; }
        int WorkerThreads { get; }
        int WriteBufferCapacity { get; }
    }
}