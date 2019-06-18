using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Implement simple thread safe queue of DataBlocks.
    /// First in first out.
    /// </summary>
    public class BlockQueue : IBlockQueue
    {
        private Queue<DataBlock> queue = new Queue<DataBlock>();
        private readonly ISettings settings;
        private readonly EventWaitHandle wh = new AutoResetEvent(false);

        public BlockQueue(ISettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Add new block to the queue.
        /// </summary>
        /// <param name="block"></param>
        public void Enqueue(DataBlock block)
        {
            Monitor.Enter(this);
            try
            {
                queue.Enqueue(block);
                wh.Set();
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Output is next data block for processing.
        /// Block is removed from queue.
        /// </summary>
        /// <param name="block"></param>
        /// <returns>True when there are no more data to process.</returns>
        public bool Dequeue(out DataBlock block)
        {
            while (true)
            {
                block = null;
                lock (this)
                    if (queue.Count > 0)
                    {
                        block = queue.Dequeue();
                        if (block == null) return true;
                    }
                if (block != null)
                {
                    return true;
                }
                else
                    wh.WaitOne();
            }
        }

        public bool IsEmpty()
        {
            Monitor.Enter(this);
            try
            {
                return queue.Count == 0;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}