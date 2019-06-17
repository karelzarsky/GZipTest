namespace GZipTest
{
    public interface IBlockDictionary
    {
        void Add(DataBlock block);
        bool TryRetrieve(long key, out DataBlock block);
        bool Empty();
    }
}