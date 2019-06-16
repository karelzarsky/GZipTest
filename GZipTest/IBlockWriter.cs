using System.IO;

namespace GZipTest
{
    public interface IBlockWriter
    {
        void WriteToStream(IBlockDictionary source, Stream destination, bool includeBlockHeader, ref long totalBlocks, IStatistics stats);
    }
}