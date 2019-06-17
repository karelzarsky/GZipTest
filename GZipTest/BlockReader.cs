using System.IO;

namespace GZipTest
{
    public class BlockReader : IBlockReader
    {
        private readonly IStatistics stats;

        public BlockReader(IStatistics stats)
        {
            this.stats = stats;
        }

        public void FillQueue(Stream stream, IBlockQueue queueEmpty, IBlockQueue queueFilled, ref long totalBlocks)
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
            totalBlocks = counter;
            queueFilled.PulseAll();
        }
    }
}
