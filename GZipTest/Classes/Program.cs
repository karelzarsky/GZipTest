using Ninject;
using System;
using System.IO;
using System.Reflection;

namespace GZipTest
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                // Ninject dependency injection container
                var kernel = new StandardKernel();
                kernel.Load(Assembly.GetExecutingAssembly());

                if (kernel.Get<IArgumentsParser>().ParseArguments(args, out string error, out Stream inputStream, out Stream outputStream))
                {
                    kernel.Get<IThreadsCreator>().StartThreads(inputStream, outputStream);
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
