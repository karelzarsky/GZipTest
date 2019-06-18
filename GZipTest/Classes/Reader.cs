using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    /// <summary>
    /// Reading thread will buffer new data blocks in advance for Workers, so they don't have to wait for I/O operation and use majority of time for data crunching.
    /// Same piecies of allocated memory are used over and over again.
    /// </summary>
    public class Reader : IReader
    {
        private readonly IStatistics stats;
        private readonly ISettings settings;
        private readonly IReadBuffer readBuffer;
        private BinaryFormatter formatter = new BinaryFormatter();

        public Reader(IStatistics stats, ISettings settings, IReadBuffer readBuffer)
        {
            this.stats = stats;
            this.settings = settings;
            this.readBuffer = readBuffer;
        }

        /// <summary>
        /// Receive data from stream, split it into blocks and store them in the queue for later processing by worker threads.
        /// </summary>
        /// <param name="stream"></param>
        public void FillQueue(Stream stream)
        {
            long counter = 0;
            int bytesRead;
            try
            {
                while (stream.CanRead)
                {
                    if (readBuffer.EmptyBlocks.Dequeue(out DataBlock block))
                    {
                        stats.DiskReadTime.Start();
                        if (settings.Mode == System.IO.Compression.CompressionMode.Decompress)
                        {
                            long seq = (long)formatter.Deserialize(stream);
                            if (seq != counter)
                                throw (new InvalidDataException());
                            int size = (int)formatter.Deserialize(stream);
                            if (size > block.Data.Length)
                            {
                                block.Data = new byte[(int)(size * 1.2)];
                            }
                            block.SequenceNr = seq;
                            block.Size = size;
                            bytesRead = stream.Read(block.Data, 0, size);
                        }
                        else
                        {
                            bytesRead = stream.Read(block.Data, 0, block.Data.Length);
                        }
                        stats.DiskReadTime.Stop();
                        if (bytesRead == 0) break;
                        block.Size = bytesRead;
                        stats.TotalBytesRead += bytesRead;
                        block.SequenceNr = counter++;
                        readBuffer.FilledBlocks.Enqueue(block);
                    }
                }
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                // End of stream reached
                // Thread can exit gracefully.
            }
            finally
            {
                stream.Dispose();
            }
            settings.TotalBlocks = counter;

            // NULL blocks signalize workers that work is complete.
            for (int i = 0; i <= settings.WorkerThreads; i++)
            {
                readBuffer.FilledBlocks.Enqueue(null);
            }
        }
    }
}
