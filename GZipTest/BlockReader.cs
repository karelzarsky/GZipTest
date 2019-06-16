using System.IO;

namespace GZipTest
{
    public class BlockReader : IBlockReader
    {
        public void FillQueue(IBlockQueue queueEmpty, IBlockQueue queueFilled, Stream stream, ref long totalBlocks, IStatistics stats)
        {
            long counter = 0;
            int bytesRead;

            while (stream.CanRead)
            {
                if (queueEmpty.TryDequeue(out DataBlock block, 100))
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
