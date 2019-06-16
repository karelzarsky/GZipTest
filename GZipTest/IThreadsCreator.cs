using System.IO;

namespace GZipTest
{
    public interface IThreadsCreator
    {
        void StartThreads(
            Stream inputStream,
            Stream outputStream, 
            IStatistics stats,
            IBlockQueue unusedSourceBlocks, 
            IBlockQueue filledSourceBlocks,
            IBlockReader blockReader, 
            IBlockWriter blockWriter,
            IBlockDictionary outputBuffer);
    }
}