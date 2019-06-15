using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class Program
    {
        const long blockSize = 1000;

        static void Main(string[] args)
        {
            int cores = Environment.ProcessorCount;

            Thread BlockReader = new Thread(_ => {});
            Thread[] Workers = new Thread[cores];
            Thread BlockWriter = new Thread(_ => {});

            var readQueue = new SafeQueue();
            for (int i = 0; i < cores; i++)
            {
                Workers[i] = new Thread(_ => { });
            }

        }
    }
}
