using System.IO;

namespace GZipTest
{
    public interface IReader
    {
        void FillQueue(Stream stream);
    }
}