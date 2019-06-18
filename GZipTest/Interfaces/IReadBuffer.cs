namespace GZipTest
{
    public interface IReadBuffer
    {
        IBlockQueue EmptyBlocks { get; }
        IBlockQueue FilledBlocks { get; }
    }
}