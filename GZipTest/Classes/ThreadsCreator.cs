using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Starts one backgroud thread for file reader.
    /// Starts several worker threads depending on CPU cores count.
    /// Handles writing to destination file itself using main thread.
    /// </summary>
    public class ThreadsCreator : IThreadsCreator
    {
        private readonly IBlockDictionary outputBuffer;
        private readonly IReader blockReader;
        private readonly IWriter blockWriter;
        private readonly IBlockQueue unusedSourceBlocks;
        private readonly IBlockQueue filledSourceBlocks;
        private readonly IStatistics stats;
        private readonly ISettings settings;
        private readonly IReadBuffer readBuffer;

        public ThreadsCreator(IBlockDictionary outputBuffer,
            IReader blockReader,
            IWriter blockWriter,
            IBlockQueue unusedSourceBlocks,
            IBlockQueue filledSourceBlocks,
            IStatistics stats,
            ISettings settings,
            IReadBuffer readBuffer)
        {
            this.outputBuffer = outputBuffer;
            this.blockReader = blockReader;
            this.blockWriter = blockWriter;
            this.unusedSourceBlocks = unusedSourceBlocks;
            this.filledSourceBlocks = filledSourceBlocks;
            this.stats = stats;
            this.settings = settings;
            this.readBuffer = readBuffer;
        }

        public void StartThreads(Stream inputStream, Stream outputStream)
        {
            stats.WriteStartMessages();
            Thread readerThread = StartReader(inputStream);
            List<Thread> workers = StartWorkers();
            blockWriter.WriteToStream(outputStream);
            readerThread.Join();
            workers.ForEach(x => x.Join());
            stats.WriteEndStatistics();
        }

        /// <summary>
        /// Background workers for data crunching.
        /// </summary>
        /// <returns></returns>
        private List<Thread> StartWorkers()
        {
            var workers = new List<Thread>();
            for (int i = 0; i < settings.WorkerThreads; i++)
            {
                unusedSourceBlocks.Enqueue(new DataBlock(settings.BlockSizeBytes));
                unusedSourceBlocks.Enqueue(new DataBlock(settings.BlockSizeBytes));
                var worker = new Thread(_ => new Worker(readBuffer, outputBuffer, stats, settings).DoCompression())
                {
                    Name = $"Worker {i}",
                    Priority = ThreadPriority.BelowNormal
                };
                workers.Add(worker);
                worker.Start();
            }
            return workers;
        }

        private Thread StartReader(Stream inputStream)
        {
            var readerThread = new Thread(_ => blockReader.FillQueue(inputStream)) { Name = "Reader" };
            readerThread.Start();
            return readerThread;
        }
    }
}
