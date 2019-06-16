using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class Worker : IWorker
    {
        Stopwatch compressionTime = new Stopwatch();
        Stopwatch inputWaitTime = new Stopwatch();
        Stopwatch outputWaitTime = new Stopwatch();

        public void DoCompression(IBlockQueue filled, IBlockQueue empty, IBlockDictionary writeDictionary, ref long totalBlocks, IStatistics stats)
        {
            while (totalBlocks == -1 || !filled.Empty())
            {
                inputWaitTime.Start();
                if (filled.TryDequeue(out DataBlock block, stats.MonitorTimeoutMilliseconds))
                {
                    inputWaitTime.Stop();
                    compressionTime.Start();
                    var compressedBlock = CompressOneBlock(block);
                    compressionTime.Stop();
                    outputWaitTime.Start();
                    writeDictionary.Add(new DataBlock(compressedBlock, block.SequenceNr));
                    outputWaitTime.Stop();
                    inputWaitTime.Start();
                    empty.Enqueue(block);
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
