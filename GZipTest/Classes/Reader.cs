using System.IO;

namespace GZipTest
{
    public class Reader : IReader
    {
        private readonly IStatistics stats;
        private readonly ISettings settings;
        private readonly IReadBuffer readBuffer;

        public Reader(IStatistics stats, ISettings settings, IReadBuffer readBuffer)
        {
            this.stats = stats;
            this.settings = settings;
            this.readBuffer = readBuffer;
        }

        public void FillQueue(Stream stream)
        {
            long counter = 0;
            int bytesRead;

            while (stream.CanRead)
            {
                if (readBuffer.EmptyBlocks.TryDequeue(out DataBlock block))
                {
                    stats.DiskReadTime.Start();
                    bytesRead = stream.Read(block.Data, 0, block.Data.Length);
                    stats.DiskReadTime.Stop();
                    if (bytesRead == 0) break;
                    block.Size = bytesRead;
                    stats.TotalBytesRead += bytesRead;
                    block.SequenceNr = counter++;
                    readBuffer.FilledBlocks.Enqueue(block);
                }
            }
            stream.Dispose();
            settings.TotalBlocks = counter;
            for (int i = 0; i <= settings.WorkerThreads; i++)
            {
                readBuffer.FilledBlocks.Enqueue(null);
            }
            readBuffer.FilledBlocks.PulseAll();
        }
    }
}
