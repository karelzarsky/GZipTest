using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GZipTest
{
    public class ThreadsCreator : IThreadsCreator
    {
        public void StartThreads(Stream inputStream, Stream outputStream, IStatistics stats, IBlockQueue unusedSourceBlocks, IBlockQueue filledSourceBlocks, IBlockReader blockReader, IBlockWriter blockWriter)
        {
            var destinationBlocks = new BlockDictionary() {MaximumCapacity = stats.WorkerThreads * 4 };
            long totalBlocks = -1;
            var readerThread = new Thread(_ => blockReader.FillQueue(unusedSourceBlocks, filledSourceBlocks, inputStream, ref totalBlocks, stats))
            { Name = "Reader" };
            readerThread.Start();
            var workers = new List<Thread>();
            for (int i = 0; i < stats.WorkerThreads; i++)
            {
                unusedSourceBlocks.Enqueue(new DataBlock(stats.BlockSizeBytes));
                unusedSourceBlocks.Enqueue(new DataBlock(stats.BlockSizeBytes));
                var worker = new Thread(_ => new Worker().DoCompression(filledSourceBlocks, unusedSourceBlocks, destinationBlocks, ref totalBlocks, stats))
                {
                    Name = $"Worker {i}",
                    Priority = ThreadPriority.BelowNormal
                };
                workers.Add(worker);
                worker.Start();
            }
            blockWriter.WriteToStream(destinationBlocks, outputStream, true, ref totalBlocks, stats);
            readerThread.Join();
            workers.ForEach(x => x.Join());
        }
    }
}
