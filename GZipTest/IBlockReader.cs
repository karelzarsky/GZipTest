using System.IO;

namespace GZipTest
{
    public interface IBlockReader
    {
        void FillQueue(ISafeQueue queueEmpty, ISafeQueue queueFiled, Stream stream);
    }
}
