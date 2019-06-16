using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class GZipWorker
    {
        public void DoCompression(IBlockQueue source, IBlockQueue used, IBlockDictionary writeDictionary, ref long totalBlocks, Statistics stat)
        {
            var inputWaitTime = new Stopwatch();
            var compressionTime = new Stopwatch();
            var outputWaitTime = new Stopwatch();
            var counter = 0L;
            long totalBytes = 0;

            while (totalBlocks == -1 || !source.Empty())
            {
                inputWaitTime.Start();
                if (source.TryDequeue(out DataBlock block, stat.timeoutMillisecond))
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
                stat.inputWaitMillisecond += inputWaitTime.ElapsedMilliseconds;
                stat.compressionTimeMillisecond += compressionTime.ElapsedMilliseconds;
                stat.outputWaitMillisecond += outputWaitTime.ElapsedMilliseconds;
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
