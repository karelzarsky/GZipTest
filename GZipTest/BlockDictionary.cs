using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    public class BlockDictionary : IBlockDictionary
    {
        private readonly Dictionary<long, DataBlock> dictionary = new Dictionary<long, DataBlock>();
        private readonly int maximumCapacity;
        private long lastRetreivedKey = -1;

        public BlockDictionary(int capacity)
        {
            maximumCapacity = capacity;
        }

        public void Add(DataBlock block)
        {
            Monitor.Enter(this);
            try
            {
                while (block.SequenceNr > lastRetreivedKey + maximumCapacity)
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

        public bool TryRetrive(long key, out DataBlock block, int timeout)
        {
            block = null;
            Monitor.Enter(this);
            try
            {
                while (!dictionary.ContainsKey(key))
                {
                    Monitor.Wait(this, timeout);
                }
                bool success = dictionary.TryGetValue(key, out block);
                dictionary.Remove(key);
                lastRetreivedKey = key;
                Monitor.PulseAll(this);
                return success;
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
                return dictionary.Count == 0;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
