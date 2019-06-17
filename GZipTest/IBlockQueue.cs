namespace GZipTest
{
    public interface IBlockQueue
    {
        bool TryDequeue(out DataBlock block);
        void Enqueue(DataBlock block);
        bool Empty();
        void PulseAll();
    }
}
