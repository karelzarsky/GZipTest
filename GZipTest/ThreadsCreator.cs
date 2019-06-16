using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GZipTest
{
    public class ThreadsCreator : IThreadsCreator
    {
        public void StartThreads(
            Stream inputStream, 
            Stream outputStream, 
            IStatistics stats, 
            IBlockQueue unusedSourceBlocks, 
            IBlockQueue filledSourceBlocks, 
            IBlockReader blockReader, 
            IBlockWriter blockWriter,
            IBlockDictionary outputBuffer)
        {
            // Reading
            long totalBlocks = -1;
            var readerThread = new Thread(_ => blockReader.FillQueue(unusedSourceBlocks, filledSourceBlocks, inputStream, ref totalBlocks, stats)) { Name = "Reader" };
            readerThread.Start();

            // Processing
            outputBuffer.MaximumCapacity = stats.WorkerThreads * 4;
            var workers = new List<Thread>();
            for (int i = 0; i < stats.WorkerThreads; i++)
            {
                unusedSourceBlocks.Enqueue(new DataBlock(stats.BlockSizeBytes));
                unusedSourceBlocks.Enqueue(new DataBlock(stats.BlockSizeBytes));
                var worker = new Thread(_ => new Worker().DoCompression(filledSourceBlocks, unusedSourceBlocks, outputBuffer, ref totalBlocks, stats))
                {
                    Name = $"Worker {i}",
                    Priority = ThreadPriority.BelowNormal
                };
                workers.Add(worker);
                worker.Start();
            }

            // Writing
            blockWriter.WriteToStream(outputBuffer, outputStream, true, ref totalBlocks, stats);

            readerThread.Join();
            workers.ForEach(x => x.Join());
        }
    }
}
