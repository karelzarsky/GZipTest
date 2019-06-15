using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    public class BlockDictionary : IBlockDictionary
    {
        private readonly Dictionary<long, DataBlock> dictionary = new Dictionary<long, DataBlock>();

        public void Add(DataBlock block)
        {
            Monitor.Enter(this);
            try
            {
                dictionary.Add(block.SequenceNr, block);
                Monitor.Pulse(this);
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
                dictionary.TryGetValue(key, out block);
                return dictionary.Remove(key);
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
