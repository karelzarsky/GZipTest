namespace GZipTest
{
    public class DataBlock
    {
        public long SequenceNr { get; set; } = -1;
        public int Size { get; set; } = 0;
        public byte[] Data { get; }

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
