using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace GZipTest
{
    class Program
    {
        const long BLOCK_SIZE = 1048576;

        static int Main(string[] args)
        {

            if (AreArgumentsValid(args, out string error, out bool compress))
            {
                Console.WriteLine($"Block size set to {BLOCK_SIZE} bytes.");
                Console.WriteLine($"Starting {Environment.ProcessorCount} compression threads.");
                var totalTime = new Stopwatch();
                totalTime.Start();
                long totalBytesRead;

                using (FileStream inputStream = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                using (FileStream outputStream = File.Create(args[2], 4096, FileOptions.WriteThrough))
                {
                    totalBytesRead = Compress(inputStream, outputStream);
                }
                Console.WriteLine($"Total time: {totalTime.ElapsedMilliseconds} ms.");
                Console.WriteLine($"Total bytes read: {totalBytesRead}.");
                Console.WriteLine($"Throughput: {totalBytesRead / totalTime.ElapsedMilliseconds / 1048.576} MB/s.");
                Console.ReadKey();
                return 0;
            }
            else
            {
                Console.WriteLine(error);
            }
            Console.ReadKey();
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

        private static long Compress(Stream inputStream, Stream outputStream)
        {
            var unusedSourceBlocks = new BlockQueue();
            var filledSourceBlocks = new BlockQueue();
            var destinationBlocks = new BlockDictionary();
            long totalBlocks = -1;
            long totalBytesRead = 0;
            var readerThread = new Thread(_ => new BlockReader().FillQueue(unusedSourceBlocks, filledSourceBlocks, inputStream, ref totalBlocks, ref totalBytesRead))
            {
                Name = "Reader"
            };
            readerThread.Start();

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                unusedSourceBlocks.Enqueue(new DataBlock(BLOCK_SIZE));
                unusedSourceBlocks.Enqueue(new DataBlock(BLOCK_SIZE));
                var worker = new Thread(_ => new GZipWorker().DoCompression(filledSourceBlocks, unusedSourceBlocks, destinationBlocks, ref totalBlocks))
                {
                    Name = $"Worker {i}"
                };
                worker.Start();
            }

            var b = new BlockWriter();
            b.WriteToStream(destinationBlocks, outputStream, true, ref totalBlocks);
            readerThread.Join();
            return totalBytesRead;
        }
    }
}
