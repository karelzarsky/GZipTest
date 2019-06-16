
namespace GZipTest
{
    public interface IWorker
    {
        byte[] CompressOneBlock(DataBlock input);
        void DoCompression(IBlockQueue source, IBlockQueue used, IBlockDictionary writeDictionary, ref long totalBlocks, IStatistics stats);
    }
}