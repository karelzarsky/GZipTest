
namespace GZipTest
{
    public interface IWorker
    {
        byte[] CompressOneBlock(DataBlock input);
        byte[] DecompressOneBlock(DataBlock input);
        void DoCompression();
    }
}