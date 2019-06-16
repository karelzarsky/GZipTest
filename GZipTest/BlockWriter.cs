using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    public class BlockWriter
    {
        const int DICTIONARY_TIMEOUT = 100;

        public void WriteToStream(IBlockDictionary source, Stream destination, bool includeBlockHeader, ref long totalBlocks, Statistics stat)
        {
            long counter = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            while (counter != totalBlocks)
            {
                if (source.TryRetrive(counter, out DataBlock block, DICTIONARY_TIMEOUT))
                {
                    if (includeBlockHeader)
                    {
                        formatter.Serialize(destination, block.SequenceNr);
                        formatter.Serialize(destination, block.Size);
                    }
                    stat.totalBytesWritten += block.Size;
                    stat.diskWriteTime.Start();
                    destination.Write(block.Data, 0, block.Size);
                    stat.diskWriteTime.Stop();
                    counter++;
                }
            }
            destination.Close();
            destination.Dispose();
        }
    }
}
