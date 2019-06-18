using System;
using System.IO.Compression;

namespace GZipTest
{
    public class Settings : ISettings
    {
        public int MonitorTimeoutMilliseconds => 100;
        public int WorkerThreads { get; set; } = Environment.ProcessorCount < 4 ? Environment.ProcessorCount : Environment.ProcessorCount - 1;
        public long BlockSizeBytes => 1048576;
        public CompressionMode Mode { get; set; } = CompressionMode.Decompress;
        public int WriteBufferCapacity => WorkerThreads * 4;
        public int ReadBufferCapacity => WorkerThreads * 4;
        public long TotalBlocks { get; set; } = -1;
    }
}
