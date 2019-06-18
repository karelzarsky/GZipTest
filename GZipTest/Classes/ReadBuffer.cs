namespace GZipTest
{
    /// <summary>
    /// Data structure holds allocated memory blocks for reading from file stream.
    /// Byte arrays are used over and over again without needing to allocate more and disposing used ones.
    /// </summary>
    public class ReadBuffer : IReadBuffer
    {
        private readonly ISettings settings;
        public IBlockQueue EmptyBlocks { get; }
        public IBlockQueue FilledBlocks { get; }

        public ReadBuffer(ISettings settings, IBlockQueue emptyBlocks, IBlockQueue filledBlocks)
        {
            this.settings = settings;
            this.EmptyBlocks = emptyBlocks;
            this.FilledBlocks = filledBlocks;
            for (int i = 0; i <= settings.ReadBufferCapacity; i++)
            {
                EmptyBlocks.Enqueue(new DataBlock(settings.BlockSizeBytes));
            }
        }

        public ReadBuffer(ISettings settings, IBlockQueue emptyBlocks, IBlockQueue filledBlocks, int capacity)
        {
            this.settings = settings;
            this.EmptyBlocks = emptyBlocks;
            this.FilledBlocks = filledBlocks;
            for (int i = 0; i < capacity; i++)
            {
                EmptyBlocks.Enqueue(new DataBlock(settings.BlockSizeBytes));
            }
        }
    }
}
