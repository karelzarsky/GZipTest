using System.IO;

namespace GZipTest
{
    public class BlockReader : IBlockReader
    {
        public void FillQueue(IBlockQueue queueEmpty, IBlockQueue queueFilled, Stream stream, ref long totalBlocks, Statistics stat)
        {
            long counter = 0;
            int bytesRead;

            while (stream.CanRead)
            {
                if (queueEmpty.TryDequeue(out DataBlock block, 100))
                {
                    stat.diskReadTime.Start();
                    bytesRead = stream.Read(block.Data, 0, block.Data.Length);
                    stat.diskReadTime.Stop();
                    if (bytesRead == 0) break;
                    block.Size = bytesRead;
                    stat.totalBytesRead += bytesRead;
                    block.SequenceNr = counter++;
                    queueFilled.Enqueue(block);
                }
            }
            stream.Dispose();
            stat.totalBlocks = totalBlocks = counter;
            queueFilled.PulseAll();
        }
    }
}
