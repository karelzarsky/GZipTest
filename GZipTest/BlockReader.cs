using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    public class BlockReader : IBlockReader
    {
        public void FillQueue(ISafeQueue queueEmpty, ISafeQueue queueFiled, Stream stream)
        {
            int counter = 0;
            int bytesRead = -1;
            while (stream.CanRead)
            {
                if (queueEmpty.TryDequeue(out DataBlock block, 100))
                {
                    bytesRead = stream.Read(block.Data, 0, block.Data.Length);
                    if (bytesRead == 0) break;
                    block.Size = bytesRead;
                    block.SequenceNr = counter++;
                    queueFiled.Enqueue(block);
                }
            }
            stream.Dispose();
        }
    }
}
