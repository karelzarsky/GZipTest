using System.IO;

namespace GZipTest
{
    public class BlockReader : IBlockReader
    {
        private readonly IStatistics stats;
        private readonly ISettings settings;

        public BlockReader(IStatistics stats, ISettings settings)
        {
            this.stats = stats;
            this.settings = settings;
        }

        public void FillQueue(Stream stream, IBlockQueue queueEmpty, IBlockQueue queueFilled)
        {
            long counter = 0;
            int bytesRead;

            while (stream.CanRead)
            {
                if (queueEmpty.TryDequeue(out DataBlock block))
                {
                    stats.DiskReadTime.Start();
                    bytesRead = stream.Read(block.Data, 0, block.Data.Length);
                    stats.DiskReadTime.Stop();
                    if (bytesRead == 0) break;
                    block.Size = bytesRead;
                    stats.TotalBytesRead += bytesRead;
                    block.SequenceNr = counter++;
                    queueFilled.Enqueue(block);
                }
            }
            stream.Dispose();
            settings.TotalBlocks = counter;
            for (int i = 0; i <= settings.WorkerThreads; i++)
            {
                queueFilled.Enqueue(null);
            }
            queueFilled.PulseAll();
        }
    }
}
