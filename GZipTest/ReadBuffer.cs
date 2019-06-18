using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
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
