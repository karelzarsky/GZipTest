using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace GZipTest
{
    class Program
    {
        const long BLOCK_SIZE = 1;

        static void Main(string[] args)
        {
            var totalTime = new Stopwatch();

            Console.WriteLine($"Block size set to {BLOCK_SIZE} bytes.");
            Console.WriteLine($"Starting {Environment.ProcessorCount} compression threads.");

            totalTime.Start();
            Compress();

            Console.WriteLine($"TOTAL PROCESSING TIME: {totalTime.ElapsedMilliseconds} ms.");
            Console.ReadKey();
        }

        private static void Compress()
        {
            var unusedSourceBlocks = new BlockQueue();
            var filledSourceBlocks = new BlockQueue();
            var destinationBlocks = new BlockDictionary();
            long totalBlocks = -1;
            var inputStream = new MemoryStream(Enumerable.Range(0, 255).Select(x => (byte)x).ToArray());
            new Thread(_ => new BlockReader().FillQueue(unusedSourceBlocks, filledSourceBlocks, inputStream, ref totalBlocks)).Start();

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                unusedSourceBlocks.Enqueue(new DataBlock(BLOCK_SIZE));
                unusedSourceBlocks.Enqueue(new DataBlock(BLOCK_SIZE));
                new Thread(_ => new GZipWorker().DoCompression(filledSourceBlocks, unusedSourceBlocks, destinationBlocks, ref totalBlocks)).Start();
            }

            var b = new BlockWriter();
            b.WriteToStream(destinationBlocks, new MemoryStream(), true, ref totalBlocks);
        }
    }
}
