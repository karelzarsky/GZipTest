
namespace GZipTest
{
    public interface IWorker
    {
        byte[] CompressOneBlock(DataBlock input);
        byte[] DecompressOneBlock(DataBlock input);
        void DoCompression(IBlockQueue source, IBlockQueue used, ref long totalBlocks);
    }
}