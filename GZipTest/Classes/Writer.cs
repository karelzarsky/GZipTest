using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    /// <summary>
    /// Handling write buffer and storing processed data to output stream in the right order.
    /// </summary>
    public class Writer : IWriter
    {
        private readonly IBlockDictionary source;
        private readonly IStatistics stats;
        private readonly ISettings settings;
        private BinaryFormatter formatter = new BinaryFormatter();

        public Writer(IBlockDictionary source, IStatistics stats, ISettings settings)
        {
            this.source = source;
            this.stats = stats;
            this.settings = settings;
        }

        /// <summary>
        /// Writes blocks to destination stream one at a time until done.
        /// </summary>
        /// <param name="destination"></param>
        public void WriteToStream(Stream destination)
        {
            try
            {
                long counter = 0;
                while (counter != settings.TotalBlocks)
                {
                    if (source.TryRetrieve(counter, out DataBlock block))
                    {
                        if (settings.Mode == System.IO.Compression.CompressionMode.Compress)
                        {
                            formatter.Serialize(destination, block.SequenceNr);
                            formatter.Serialize(destination, block.Size);
                        }
                        stats.TotalBytesWritten += block.Size;
                        stats.DiskWriteTime.Start();
                        destination.Write(block.Data, 0, block.Size);
                        stats.DiskWriteTime.Stop();
                        counter++;
                    }
                    stats.WriteEarlyStatistics();
                }
            }
            finally
            {
                destination.Dispose();
            }
        }
    }
}
