using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    /// <summary>
    /// Main data processing.
    /// Measures time spent computing and waiting for data.
    /// </summary>
    public class Worker : IWorker
    {
        Stopwatch processingTime = new Stopwatch();
        Stopwatch inputWaitTime = new Stopwatch();
        Stopwatch outputWaitTime = new Stopwatch();
        private readonly IBlockDictionary writeDictionary;
        private readonly IStatistics stats;
        private readonly ISettings settings;
        private readonly IReadBuffer readBuffer;

        public Worker(IReadBuffer readBuffer, IBlockDictionary writeBuffer, IStatistics stats, ISettings settings)
        {
            this.writeDictionary = writeBuffer;
            this.stats = stats;
            this.settings = settings;
            this.readBuffer = readBuffer;
        }

        /// <summary>
        /// Retrive one block from read queue.
        /// Compress block.
        /// Send it to buffer for writing to file.
        /// Repeat until everithing is processed.
        /// </summary>
        public void DoCompression()
        {
            inputWaitTime.Start();
            while (readBuffer.FilledBlocks.Dequeue(out DataBlock block))
            {
                if (block == null) break;
                inputWaitTime.Stop();
                processingTime.Start();
                var compressedBlock = CompressOneBlock(block);
                processingTime.Stop();
                outputWaitTime.Start();
                writeDictionary.Add(new DataBlock(compressedBlock, block.SequenceNr));
                outputWaitTime.Stop();
                inputWaitTime.Start();
                readBuffer.EmptyBlocks.Enqueue(block);
            }
            inputWaitTime.Stop();
            lock (stats)
            {
                stats.InputWaitMilliseconds += inputWaitTime.ElapsedMilliseconds;
                stats.CompressionTimeMilliseconds += processingTime.ElapsedMilliseconds;
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
