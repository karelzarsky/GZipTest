namespace GZipTest
{
    public interface IBlockQueue
    {
        bool TryDequeue(out DataBlock block, int timeout);
        void Enqueue(DataBlock block);
        bool Empty();
        void PulseAll();
    }
}
