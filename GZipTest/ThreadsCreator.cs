using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GZipTest
{
    public class ThreadsCreator : IThreadsCreator
    {
        private readonly IBlockDictionary outputBuffer;
        private readonly IBlockReader blockReader;
        private readonly IBlockWriter blockWriter;
        private readonly IBlockQueue unusedSourceBlocks;
        private readonly IBlockQueue filledSourceBlocks;
        private readonly IStatistics stats;
        private readonly ISettings settings;

        public ThreadsCreator(IBlockDictionary outputBuffer,
            IBlockReader blockReader,
            IBlockWriter blockWriter,
            IBlockQueue unusedSourceBlocks,
            IBlockQueue filledSourceBlocks,
            IStatistics stats,
            ISettings settings)
        {
            this.outputBuffer = outputBuffer;
            this.blockReader = blockReader;
            this.blockWriter = blockWriter;
            this.unusedSourceBlocks = unusedSourceBlocks;
            this.filledSourceBlocks = filledSourceBlocks;
            this.stats = stats;
            this.settings = settings;
        }

        public void StartThreads(Stream inputStream, Stream outputStream)
        {
            stats.WriteStartMessages();

            // Reading
            long totalBlocks = -1;
            var readerThread = new Thread(_ => blockReader.FillQueue(inputStream, unusedSourceBlocks, filledSourceBlocks, ref totalBlocks)) { Name = "Reader" };
            readerThread.Start();

            // Processing
            
            var workers = new List<Thread>();
            for (int i = 0; i < settings.WorkerThreads; i++)
            {
                unusedSourceBlocks.Enqueue(new DataBlock(settings.BlockSizeBytes));
                unusedSourceBlocks.Enqueue(new DataBlock(settings.BlockSizeBytes));
                var worker = new Thread(_ => new Worker(outputBuffer, stats, settings).DoCompression(filledSourceBlocks, unusedSourceBlocks, ref totalBlocks))
                {
                    Name = $"Worker {i}",
                    Priority = ThreadPriority.BelowNormal
                };
                workers.Add(worker);
                worker.Start();
            }

            // Writing
            blockWriter.WriteToStream(outputStream, ref totalBlocks);

            readerThread.Join();
            workers.ForEach(x => x.Join());
            stats.WriteEndStatistics();
        }
    }
}
