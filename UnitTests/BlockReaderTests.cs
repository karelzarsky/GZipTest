using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System.IO;
using Ninject;

namespace UnitTests
{
    [TestClass]
    public class BlockReaderTests
    {
        StandardKernel kernel = new StandardKernel();

        public BlockReaderTests()
        {
            kernel.Load(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        public void BlockReader_FillQueue_RemovesItemsFromInputQueue()
        {
            var inputQueue = kernel.Get<IBlockQueue>();
            var outputQueue = kernel.Get<IBlockQueue>();
            var sut = kernel.Get<IBlockReader>();
            var stream = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7});
            inputQueue.Enqueue(new DataBlock(100));
            inputQueue.Enqueue(new DataBlock(100));

            sut.FillQueue(stream, inputQueue, outputQueue);
            bool canDequeue = inputQueue.TryDequeue(out DataBlock b);

            Assert.AreEqual(false, canDequeue);
        }

        [TestMethod]
        public void BlockReader_FillQueue_AddsItemsToOutputQueue()
        {
            var inputQueue = kernel.Get<IBlockQueue>();
            var outputQueue = kernel.Get<IBlockQueue>();
            var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            inputQueue.Enqueue(new DataBlock(100));
            inputQueue.Enqueue(new DataBlock(100));
            var sut = kernel.Get<IBlockReader>();

            sut.FillQueue(stream, inputQueue, outputQueue);
            bool canDequeue = outputQueue.TryDequeue(out DataBlock b);

            Assert.AreEqual(true, canDequeue);
        }

        [TestMethod]
        public void BlockReader_FillQueue_FillsDataFromStream()
        {
            var inputQueue = kernel.Get<IBlockQueue>();
            var outputQueue = kernel.Get<IBlockQueue>();
            var sut = kernel.Get<IBlockReader>();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            inputQueue.Enqueue(new DataBlock(100));
            inputQueue.Enqueue(new DataBlock(100));

            sut.FillQueue(stream, inputQueue, outputQueue);
            bool canDequeue = outputQueue.TryDequeue(out DataBlock b);

            Assert.AreEqual(0, b.Data[0]);
            Assert.AreEqual(1, b.Data[1]);
            Assert.AreEqual(2, b.Data[2]);
            Assert.AreEqual(3, b.Data[3]);
        }

        [TestMethod]
        public void BlockReader_FillQueue_SplitsDataToBlocks()
        {
            var inputQueue = kernel.Get<IBlockQueue>();
            var outputQueue = kernel.Get<IBlockQueue>();
            var sut = kernel.Get<IBlockReader>();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                inputQueue.Enqueue(new DataBlock(5));
            }

            sut.FillQueue(stream, inputQueue, outputQueue);
            bool canDequque1 = outputQueue.TryDequeue(out DataBlock b1);
            bool canDequque2 = outputQueue.TryDequeue(out DataBlock b2);
            bool canDequque3 = outputQueue.TryDequeue(out DataBlock b3);

            Assert.AreEqual(true, canDequque1);
            Assert.AreEqual(true, canDequque2);
            Assert.AreEqual(false, canDequque3);
            Assert.AreEqual(7, b2.Data[2]);
        }

        [TestMethod]
        public void BlockReader_FillQueue_CountsSequenceNumbers()
        {
            var inputQueue = kernel.Get<IBlockQueue>();
            var outputQueue = kernel.Get<IBlockQueue>();
            var sut = kernel.Get<IBlockReader>();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                inputQueue.Enqueue(new DataBlock(1));
            }

            sut.FillQueue(stream, inputQueue, outputQueue);
            bool canDequque1 = outputQueue.TryDequeue(out DataBlock b1);
            bool canDequque2 = outputQueue.TryDequeue(out DataBlock b2);
            bool canDequque3 = outputQueue.TryDequeue(out DataBlock b3);

            Assert.AreEqual(0, b1.SequenceNr);
            Assert.AreEqual(1, b2.SequenceNr);
            Assert.AreEqual(2, b3.SequenceNr);
        }

        [TestMethod]
        public void BlockReader_FillQueue_CountsBlockSize()
        {
            var inputQueue = kernel.Get<IBlockQueue>();
            var outputQueue = kernel.Get<IBlockQueue>();
            var sut = kernel.Get<IBlockReader>();
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                inputQueue.Enqueue(new DataBlock(5));
            }

            sut.FillQueue(stream, inputQueue, outputQueue);
            bool canDequque1 = outputQueue.TryDequeue(out DataBlock b1);
            bool canDequque2 = outputQueue.TryDequeue(out DataBlock b2);
            bool canDequque3 = outputQueue.TryDequeue(out DataBlock b3);

            Assert.AreEqual(5, b1.Size);
            Assert.AreEqual(3, b2.Size);
        }
    }
}
