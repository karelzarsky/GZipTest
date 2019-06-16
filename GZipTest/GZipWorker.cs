using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class GZipWorker
    {
        long totalBytes = 0L;
        Stopwatch compressionTime = new Stopwatch();
        Stopwatch inputWaitTime = new Stopwatch();
        Stopwatch outputWaitTime = new Stopwatch();
        long counter = 0L;

        public void DoCompression(IBlockQueue source, IBlockQueue used, IBlockDictionary writeDictionary, ref long totalBlocks, Statistics stat)
        {
            while (totalBlocks == -1 || !source.Empty())
            {
                inputWaitTime.Start();
                if (source.TryDequeue(out DataBlock block, stat.timeoutMilliseconds))
                {
                    inputWaitTime.Stop();
                    compressionTime.Start();
                    var compressedBlock = CompressOneBlock(block);
                    compressionTime.Stop();
                    totalBytes += block.Size;
                    outputWaitTime.Start();
                    writeDictionary.Add(new DataBlock(compressedBlock, block.SequenceNr));
                    outputWaitTime.Stop();
                    inputWaitTime.Start();
                    used.Enqueue(block);
                    inputWaitTime.Stop();
                    counter++;
                }
            }
            lock (stat)
            {
                stat.inputWaitMilliseconds += inputWaitTime.ElapsedMilliseconds;
                stat.compressionTimeMilliseconds += compressionTime.ElapsedMilliseconds;
                stat.outputWaitMilliseconds += outputWaitTime.ElapsedMilliseconds;
            }
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
