using System;
using System.Diagnostics;
using System.IO;

namespace GZipTest
{
    public class BlockReader : IBlockReader
    {
        public void FillQueue(IBlockQueue queueEmpty, IBlockQueue queueFilled, Stream stream, ref long totalBlocks, ref long totalBytesRead)
        {
            Console.WriteLine($"[Thread R {Environment.CurrentManagedThreadId}] Reading started.");
            long counter = 0;
            int bytesRead;
            var waitTime = new Stopwatch();
            var readTime = new Stopwatch();
            var enqueueTime = new Stopwatch();

            while (stream.CanRead)
            {
                waitTime.Start();
                if (queueEmpty.TryDequeue(out DataBlock block, 100))
                {
                    waitTime.Stop();
                    readTime.Start();
                    bytesRead = stream.Read(block.Data, 0, block.Data.Length);
                    if (bytesRead == 0) break;
                    block.Size = bytesRead;
                    totalBytesRead += bytesRead;
                    block.SequenceNr = counter++;
                    readTime.Stop();
                    enqueueTime.Start();
                    queueFilled.Enqueue(block);
                    enqueueTime.Stop();
                }
            }
            stream.Dispose();
            totalBlocks = counter;
            Console.WriteLine($"[Thread R {Environment.CurrentManagedThreadId}] Read {totalBlocks} blocks. " +
            $"Wait:{waitTime.ElapsedMilliseconds} ms, Read:{readTime.ElapsedMilliseconds} ms, Enqueue:{enqueueTime.ElapsedMilliseconds} ms");
            queueFilled.PulseAll();
        }
    }
}
