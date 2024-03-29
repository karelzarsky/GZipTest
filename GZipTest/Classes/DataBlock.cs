﻿namespace GZipTest
{
    public class DataBlock : IDataBlock
    {
        public long SequenceNr { get; set; } = -1;
        public int Size { get; set; } = 0;
        public byte[] Data { get; set; }

        public DataBlock(long size)
        {
            Data = new byte[size];
        }

        public DataBlock(byte[] array, long sequenceNr)
        {
            Data = array;
            Size = array.Length;
            SequenceNr = sequenceNr;
        }
    }
}
