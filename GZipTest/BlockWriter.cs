using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    public class BlockWriter
    {
        const int DICTIONARY_TIMEOUT = 100;

        public void WriteToStream(IBlockDictionary source, Stream destination, bool includeBlockHeader, ref long totalBlocks)
        {
            Console.WriteLine($"[Thread W {Environment.CurrentManagedThreadId}] Writing started.");
            long counter = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            var writeTime = new Stopwatch();
            while (counter != totalBlocks)
            {
                if (source.TryRetrive(counter, out DataBlock block, DICTIONARY_TIMEOUT))
                {
                    if (includeBlockHeader)
                    {
                        formatter.Serialize(destination, block.SequenceNr);
                        formatter.Serialize(destination, block.Size);
                    }
                    writeTime.Start();
                    destination.Write(block.Data, 0, block.Size);
                    writeTime.Stop();
                    //Console.WriteLine($"[Thread W {Environment.CurrentManagedThreadId}] Written block {block.SequenceNr}.");
                    counter++;
                }
            }
            Console.WriteLine($"[Thread W {Environment.CurrentManagedThreadId}] Writing {counter} blocks took {writeTime.ElapsedMilliseconds} ms.");
        }
    }
}
