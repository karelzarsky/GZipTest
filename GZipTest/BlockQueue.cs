using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    public class BlockQueue : IBlockQueue
    {
        private Queue<DataBlock> queue = new Queue<DataBlock>();

        public void Enqueue(DataBlock block)
        {
            Monitor.Enter(this);
            try
            {
                queue.Enqueue(block);
                Monitor.Pulse(this);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public bool TryDequeue(out DataBlock block, int timeout)
        {
            block = null;
            Monitor.Enter(this);
            try
            {
                if (queue.Count == 0)
                {
                    Monitor.Wait(this, timeout);
                }
                try
                {
                    if (queue.Count > 0)
                    {
                        block = queue.Dequeue();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (System.InvalidOperationException)
                {
                    return false;
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public bool Empty()
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

        public void PulseAll()
        {
            Monitor.Enter(this);
            try
            {
                Monitor.PulseAll(this);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}