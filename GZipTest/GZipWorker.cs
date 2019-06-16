using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class GZipWorker : IGZipWorker
    {
        long totalBytes = 0L;
        Stopwatch compressionTime = new Stopwatch();
        Stopwatch inputWaitTime = new Stopwatch();
        Stopwatch outputWaitTime = new Stopwatch();

        public void DoCompression(IBlockQueue source, IBlockQueue used, IBlockDictionary writeDictionary, ref long totalBlocks, IStatistics stats)
        {
            while (totalBlocks == -1 || !source.Empty())
            {
                inputWaitTime.Start();
                if (source.TryDequeue(out DataBlock block, stats.TimeoutMilliseconds))
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
                }
                inputWaitTime.Stop();
            }
            lock (stats)
            {
                stats.InputWaitMilliseconds += inputWaitTime.ElapsedMilliseconds;
                stats.CompressionTimeMilliseconds += compressionTime.ElapsedMilliseconds;
                stats.OutputWaitMilliseconds += outputWaitTime.ElapsedMilliseconds;
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
