namespace GZipTest
{
    public interface IGZipWorker
    {
        byte[] CompressOneBlock(DataBlock input);
        void DoCompression(IBlockQueue source, IBlockQueue used, IBlockDictionary writeDictionary, ref long totalBlocks, IStatistics stats);
    }
}