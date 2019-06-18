using System.IO;

namespace GZipTest
{
    public interface IWriter
    {
        void WriteToStream(Stream destination);
    }
}