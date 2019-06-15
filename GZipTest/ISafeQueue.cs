using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    public interface ISafeQueue
    {
        bool TryDequeue(out DataBlock block, int timeout);
        void Enqueue(DataBlock block);
    }
}
