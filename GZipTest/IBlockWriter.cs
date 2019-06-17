using System.IO;

namespace GZipTest
{
    public interface IBlockWriter
    {
        void WriteToStream(Stream destination);
    }
}