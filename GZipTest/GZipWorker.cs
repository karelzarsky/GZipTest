using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class GZipWorker
    {
        const int QUEUE_TIMEOUT = 100;

        public void DoCompression(IBlockQueue source, IBlockQueue used, IBlockDictionary writeDictionary, ref long totalBlocks)
        {
            Console.WriteLine($"[Thread C {Environment.CurrentManagedThreadId}] Compression thread started.");

            var dequeueTime = new Stopwatch();
            var compressionTime = new Stopwatch();
            var dictionaryAddTime = new Stopwatch();
            var counter = 0L;

            while (totalBlocks == -1 || !source.Empty())
            {
                dequeueTime.Start();
                if (source.TryDequeue(out DataBlock block, QUEUE_TIMEOUT))
                {
                    dequeueTime.Stop();
                    compressionTime.Start();
                    var compressedBlock = CompressOneBlock(block);
                    compressionTime.Stop();
                    dictionaryAddTime.Start();
                    writeDictionary.Add(new DataBlock(compressedBlock, block.SequenceNr));
                    used.Enqueue(block);
                    dictionaryAddTime.Stop();
                    //Console.WriteLine($"[Thread C {Environment.CurrentManagedThreadId}] Processing block {block.SequenceNr}. Size: {block.Size} -> {compressedBlock.Length}");
                    counter++;
                }
            }
            Console.WriteLine($"[Thread C {Environment.CurrentManagedThreadId}] Processed {counter} blocks. " + 
            $"Wait+Dequeue:{dequeueTime.ElapsedMilliseconds} ms, " +
            $"Compress:{compressionTime.ElapsedMilliseconds} ms, " +
            $"Dictionary:{dictionaryAddTime.ElapsedMilliseconds} ms");
        }

        public byte[] CompressOneBlock (DataBlock input)
        {
            using (var output = new MemoryStream())
            using (var gs = new GZipStream(output, CompressionMode.Compress))
            {
                gs.Write(input.Data, 0, input.Size);
                gs.Close();
                return output.ToArray();
            }
        }
    }
}
