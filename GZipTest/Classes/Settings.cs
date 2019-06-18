using System;
using System.IO.Compression;

namespace GZipTest
{
    /// <summary>
    /// Global settings values
    /// </summary>
    public class Settings : ISettings
    {
        public int MonitorTimeoutMilliseconds => 100;
        public int WorkerThreads { get; } = Environment.ProcessorCount < 4 ? Environment.ProcessorCount : Environment.ProcessorCount - 1;
        public long BlockSizeBytes => 1048576;
        public CompressionMode Mode { get; set; }
        public int WriteBufferCapacity => WorkerThreads * 2;
        public int ReadBufferCapacity => WorkerThreads * 2;
        public long TotalBlocks { get; set; } = -1;
    }
}
