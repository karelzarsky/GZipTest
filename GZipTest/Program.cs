using System;
using System.IO;
using System.Threading;

namespace GZipTest
{
    class Program
    {
        static int Main(string[] args)
        {
            var stat = new Statistics();

            if (AreArgumentsValid(args, out string error, out bool compress))
            {
                stat.WriteStartMessages();
                using (FileStream inputStream = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                using (FileStream outputStream = File.Create(args[2], 4096, FileOptions.WriteThrough))
                {
                    Compress(inputStream, outputStream, stat);
                }
                stat.WriteEndStatistics();
                return 0;
            }
            else
            {
                Console.WriteLine(error);
            }
            return 1;
        }

        private static bool TryOpenFiles(string[] args, out Stream input, out Stream output)
        {
            input = null;
            output = null;
            return true;
        }

        private static bool AreArgumentsValid(string[] args, out string ErrorMessage, out bool compress)
        {
            ErrorMessage = null;
            compress = false;
            if (args.Length == 3)
            {
                if (args[0].Equals("compress", StringComparison.OrdinalIgnoreCase))
                {
                    compress = true;
                    return true;
                }
                if (args[0].Equals("decompress", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            ErrorMessage = "Invalid arguments!\r\n" +
            "- compressing: GZipTest.exe compress [original file name] [archive file name]\r\n" +
            "- decompressing: GZipTest.exe decompress [archive file name] [decompressing file name]\r\n";
            return false;
        }

        private static void Compress(Stream inputStream, Stream outputStream, Statistics stat)
        {
            var unusedSourceBlocks = new BlockQueue();
            var filledSourceBlocks = new BlockQueue();
            var destinationBlocks = new BlockDictionary(stat.workerThreads * 4);
            long totalBlocks = -1;
            var readerThread = new Thread(_ => new BlockReader().FillQueue(unusedSourceBlocks, filledSourceBlocks, inputStream, ref totalBlocks, stat))
            {
                Name = "Reader"
            };
            readerThread.Start();

            for (int i = 0; i < stat.workerThreads; i++)
            {
                unusedSourceBlocks.Enqueue(new DataBlock(stat.blockSizeBytes));
                unusedSourceBlocks.Enqueue(new DataBlock(stat.blockSizeBytes));
                var worker = new Thread(_ => new GZipWorker().DoCompression(filledSourceBlocks, unusedSourceBlocks, destinationBlocks, ref totalBlocks, stat))
                {
                    Name = $"Worker {i}",
                    Priority = ThreadPriority.BelowNormal
                };
                worker.Start();
            }

            var b = new BlockWriter();
            b.WriteToStream(destinationBlocks, outputStream, true, ref totalBlocks, stat);
            readerThread.Join();
        }
    }
}
