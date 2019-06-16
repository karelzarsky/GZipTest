using System;
using System.IO;
using System.Reflection;
using Ninject;

namespace GZipTest
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var kernel = new StandardKernel();
                kernel.Load(Assembly.GetExecutingAssembly());
                var stats = kernel.Get<IStatistics>();
                if (kernel.Get<IArgumentsParser>().ParseArguments(args, out string error, out bool compress, out Stream inputStream, out Stream outputStream))
                {
                    stats.WriteStartMessages();
                    kernel.Get<IThreadsCreator>().StartThreads(
                        inputStream: inputStream,
                        outputStream: outputStream,
                        stats: stats,
                        unusedSourceBlocks: kernel.Get<IBlockQueue>(),
                        filledSourceBlocks: kernel.Get<IBlockQueue>(),
                        blockReader: kernel.Get<IBlockReader>(),
                        blockWriter: kernel.Get<IBlockWriter>());
                    stats.WriteEndStatistics();
                    return 0;
                }
                Console.WriteLine(error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 1;
        }
    }
}
