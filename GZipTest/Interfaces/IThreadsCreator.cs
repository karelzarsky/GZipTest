using System.IO;

namespace GZipTest
{
    public interface IThreadsCreator
    {
        void StartThreads(Stream inputStream, Stream outputStream);
    }
}