namespace GZipTest
{
    public interface IBlockQueue
    {
        bool Dequeue(out DataBlock block);
        void Enqueue(DataBlock block);
        bool IsEmpty();
    }
}
