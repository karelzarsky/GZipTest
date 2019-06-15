using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    public class SafeQueue : ISafeQueue
    {
        private Queue<DataBlock> _queue = new Queue<DataBlock>();

        public void Enqueue(DataBlock block)
        {
            lock (_queue)
            {
                _queue.Enqueue(block);
                Monitor.Pulse(_queue);
            }
        }

        public bool TryDequeue(out DataBlock block, int timeout)
        {
            block = null;
            lock (_queue)
            {
                if (_queue.Count == 0)
                {
                    Monitor.Wait(_queue, timeout);
                }
                try
                {
                    block = _queue.Dequeue();
                    return true;
                }
                catch (System.InvalidOperationException)
                {
                    return false;
                }
            }
        }
    }
}