using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class BlockReaderTests
    {
        [TestMethod]
        public void BlockReader_FillQueue_RemovesItemsFromInputQueue()
        {
            var sut = new BlockReader();
            var inputQueue = new SafeQueue();
            var outputQueue = new SafeQueue();
            var stream = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7});
            inputQueue.Enqueue(new DataBlock(100));
            inputQueue.Enqueue(new DataBlock(100));

            sut.FillQueue(inputQueue, outputQueue, stream);
            bool canDequeue = inputQueue.TryDequeue(out DataBlock b, 100);

            Assert.AreEqual(false, canDequeue);
        }

        [TestMethod]
        public void BlockReader_FillQueue_AddsItemsToOutputQueue()
        {
            var sut = new BlockReader();
            var inputQueue = new SafeQueue();
            var outputQueue = new SafeQueue();
            var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            inputQueue.Enqueue(new DataBlock(100));
            inputQueue.Enqueue(new DataBlock(100));

            sut.FillQueue(inputQueue, outputQueue, stream);
            bool canDequeue = outputQueue.TryDequeue(out DataBlock b, 100);

            Assert.AreEqual(true, canDequeue);
        }

        [TestMethod]
        public void BlockReader_FillQueue_FillsDataFromStream()
        {
            var sut = new BlockReader();
            var inputQueue = new SafeQueue();
            var outputQueue = new SafeQueue();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            inputQueue.Enqueue(new DataBlock(100));
            inputQueue.Enqueue(new DataBlock(100));

            sut.FillQueue(inputQueue, outputQueue, stream);
            bool canDequeue = outputQueue.TryDequeue(out DataBlock b, 100);

            Assert.AreEqual(0, b.Data[0]);
            Assert.AreEqual(1, b.Data[1]);
            Assert.AreEqual(2, b.Data[2]);
            Assert.AreEqual(3, b.Data[3]);
        }

        [TestMethod]
        public void BlockReader_FillQueue_SplitsDataToBlocks()
        {
            var sut = new BlockReader();
            var inputQueue = new SafeQueue();
            var outputQueue = new SafeQueue();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                inputQueue.Enqueue(new DataBlock(5));
            }

            sut.FillQueue(inputQueue, outputQueue, stream);
            bool canDequque1 = outputQueue.TryDequeue(out DataBlock b1, 100);
            bool canDequque2 = outputQueue.TryDequeue(out DataBlock b2, 100);
            bool canDequque3 = outputQueue.TryDequeue(out DataBlock b3, 100);

            Assert.AreEqual(true, canDequque1);
            Assert.AreEqual(true, canDequque2);
            Assert.AreEqual(false, canDequque3);
            Assert.AreEqual(7, b2.Data[2]);
        }

        [TestMethod]
        public void BlockReader_FillQueue_CountsSequenceNumbers()
        {
            var sut = new BlockReader();
            var inputQueue = new SafeQueue();
            var outputQueue = new SafeQueue();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                inputQueue.Enqueue(new DataBlock(1));
            }

            sut.FillQueue(inputQueue, outputQueue, stream);
            bool canDequque1 = outputQueue.TryDequeue(out DataBlock b1, 100);
            bool canDequque2 = outputQueue.TryDequeue(out DataBlock b2, 100);
            bool canDequque3 = outputQueue.TryDequeue(out DataBlock b3, 100);

            Assert.AreEqual(0, b1.SequenceNr);
            Assert.AreEqual(1, b2.SequenceNr);
            Assert.AreEqual(2, b3.SequenceNr);
        }

        [TestMethod]
        public void BlockReader_FillQueue_CountsBlockSize()
        {
            var sut = new BlockReader();
            var inputQueue = new SafeQueue();
            var outputQueue = new SafeQueue();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                inputQueue.Enqueue(new DataBlock(5));
            }

            sut.FillQueue(inputQueue, outputQueue, stream);
            bool canDequque1 = outputQueue.TryDequeue(out DataBlock b1, 100);
            bool canDequque2 = outputQueue.TryDequeue(out DataBlock b2, 100);
            bool canDequque3 = outputQueue.TryDequeue(out DataBlock b3, 100);

            Assert.AreEqual(5, b1.Size);
            Assert.AreEqual(3, b2.Size);
        }
    }
}
