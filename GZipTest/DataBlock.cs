using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    public class DataBlock
    {
        public long SequenceNr { get; set; } = -1;
        public long Size { get; set; } = 0;
        public byte[] Data { get; }

        public DataBlock(long size)
        {
            Data = new byte[size];
        }
    }
}
