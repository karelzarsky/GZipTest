using System.IO;

namespace GZipTest
{
    public interface IBlockReader
    {
        void FillQueue(IBlockQueue queueEmpty, IBlockQueue queueFiled, Stream stream, ref long totalBlocks);
    }
}
