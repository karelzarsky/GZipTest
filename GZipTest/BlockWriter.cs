using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    public class BlockWriter : IBlockWriter
    {
        public void WriteToStream(IBlockDictionary source, Stream destination, bool includeBlockHeader, ref long totalBlocks, IStatistics stats)
        {
            long counter = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            while (counter != totalBlocks)
            {
                if (source.TryRetrive(counter, out DataBlock block, stats.MonitorTimeoutMilliseconds))
                {
                    if (includeBlockHeader)
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
