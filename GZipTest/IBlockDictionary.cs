namespace GZipTest
{
    public interface IBlockDictionary
    {
        int MaximumCapacity { get; set; }
        void Add(DataBlock block);
        bool TryRetrive(long key, out DataBlock block, int timeout);
        bool Empty();
    }
}