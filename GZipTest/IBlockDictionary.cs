namespace GZipTest
{
    public interface IBlockDictionary
    {
        void Add(DataBlock block);
        bool TryRetrive(long key, out DataBlock block);
        bool Empty();
    }
}