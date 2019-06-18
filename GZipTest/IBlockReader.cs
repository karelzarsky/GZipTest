using System.IO;

namespace GZipTest
{
    public interface IBlockReader
    {
        void FillQueue(Stream stream);
    }
}