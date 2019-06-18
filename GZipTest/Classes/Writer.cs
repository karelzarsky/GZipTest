using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    public class Writer : IWriter
    {
        private readonly IBlockDictionary source;
        private readonly IStatistics stats;
        private readonly ISettings settings;

        public Writer(IBlockDictionary source, IStatistics stats, ISettings settings)
        {
            this.source = source;
            this.stats = stats;
            this.settings = settings;
        }

        public void WriteToStream(Stream destination)
        {
            long counter = 0;
            BinaryFormatter formatter = new BinaryFormatter();
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
            destination.Close();
        }
    }
}
