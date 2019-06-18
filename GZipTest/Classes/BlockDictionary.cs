using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Simple thread safe buffer for writing data block in the right order.
    /// </summary>
    public class BlockDictionary : IBlockDictionary
    {
        private readonly Dictionary<long, DataBlock> dictionary = new Dictionary<long, DataBlock>();
        private readonly ISettings settings;
        private long lastRetrievedKey = -1;

        public BlockDictionary(ISettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Add new block to buffer.
        /// If there is too much data, it will wait.
        /// </summary>
        public void Add(DataBlock block)
        {
            Monitor.Enter(this);
            try
            {
                while (block.SequenceNr > lastRetrievedKey + settings.WriteBufferCapacity)
                {
                    Monitor.Wait(this);
                }
                dictionary.Add(block.SequenceNr, block);
                Monitor.PulseAll(this);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Outputs next data block in sequence.
        /// If it is not present yet, it will wait.
        /// </summary>
        public bool TryRetrieve(long key, out DataBlock block)
        {
            block = null;
            Monitor.Enter(this);
            try
            {
                while (!dictionary.ContainsKey(key) && key < settings.TotalBlocks)
                {
                    Monitor.Wait(this, settings.MonitorTimeoutMilliseconds);
                }
                bool success = dictionary.TryGetValue(key, out block);
                if (success)
                {
                    dictionary.Remove(key);
                    lastRetrievedKey = key;
                    Monitor.PulseAll(this);
                }
                return success;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        public bool IsEmpty()
        {
            Monitor.Enter(this);
            try
            {
                return dictionary.Count == 0;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
