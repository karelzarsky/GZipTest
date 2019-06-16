namespace GZipTest
{
    public interface IDataBlock
    {
        byte[] Data { get; }
        long SequenceNr { get; set; }
        int Size { get; set; }
    }
}