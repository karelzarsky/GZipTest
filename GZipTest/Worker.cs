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
        private readonly IBlockDictionary writeDictionary;
        private readonly IStatistics stats;
        private readonly ISettings settings;

        public Worker(IBlockDictionary writeDictionary, IStatistics stats, ISettings settings)
        {
            this.writeDictionary = writeDictionary;
            this.stats = stats;
            this.settings = settings;
        }

        public void DoCompression(IBlockQueue filled, IBlockQueue empty)
        {
            while (true)
            {
                inputWaitTime.Start();
                if (filled.TryDequeue(out DataBlock block))
                {
                    if (block == null) break;
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

        public byte[] DecompressOneBlock(DataBlock input)
        {
            using (var res = new MemoryStream())
            using (var original = new MemoryStream(input.Data))
            using (GZipStream decompressionStream = new GZipStream(original, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(res);
                return res.ToArray();
            }
        }
    }
}
