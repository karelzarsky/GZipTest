using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using System.IO;
using Ninject;

namespace UnitTests
{
    [TestClass]
    public class ReaderTests
    {
        StandardKernel kernel = new StandardKernel();

        public ReaderTests()
        {
            kernel.Load(System.Reflection.Assembly.GetExecutingAssembly());
        }

        private void PrepareReader(out IReadBuffer readBuffer, out IReader sut)
        {
            readBuffer = new ReadBuffer(kernel.Get<ISettings>(), kernel.Get<IBlockQueue>(), kernel.Get<IBlockQueue>(), 0);
            sut = new Reader(kernel.Get<IStatistics>(), kernel.Get<ISettings>(), readBuffer);
        }

        [TestMethod]
        public void BlockReader_FillQueue_RemovesItemsFromEmptyBlocks()
        {
            PrepareReader(out IReadBuffer readBuffer, out IReader sut);
            var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            readBuffer.EmptyBlocks.Enqueue(new DataBlock(100));
            readBuffer.EmptyBlocks.Enqueue(new DataBlock(100));

            sut.FillQueue(stream);
            bool canDequeue = readBuffer.EmptyBlocks.TryDequeue(out DataBlock b);

            Assert.AreEqual(false, canDequeue);
        }

        [TestMethod]
        public void BlockReader_FillQueue_AddsItemsToFilledBlocks()
        {
            PrepareReader(out IReadBuffer readBuffer, out IReader sut);
            var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7 });
            readBuffer.EmptyBlocks.Enqueue(new DataBlock(100));
            readBuffer.EmptyBlocks.Enqueue(new DataBlock(100));

            sut.FillQueue(stream);
            bool canDequeue = readBuffer.FilledBlocks.TryDequeue(out DataBlock b);

            Assert.AreEqual(true, canDequeue);
        }

        [TestMethod]
        public void BlockReader_FillQueue_FillsDataFromStream()
        {
            PrepareReader(out IReadBuffer readBuffer, out IReader sut);
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            readBuffer.EmptyBlocks.Enqueue(new DataBlock(100));
            readBuffer.EmptyBlocks.Enqueue(new DataBlock(100));

            sut.FillQueue(stream);
            bool canDequeue = readBuffer.FilledBlocks.TryDequeue(out DataBlock b);

            Assert.AreEqual(0, b.Data[0]);
            Assert.AreEqual(1, b.Data[1]);
            Assert.AreEqual(2, b.Data[2]);
            Assert.AreEqual(3, b.Data[3]);
        }

        [TestMethod]
        public void BlockReader_FillQueueCoplete_AddsNullBlockToFilledBlocks()
        {
            PrepareReader(out IReadBuffer readBuffer, out IReader sut);
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                readBuffer.EmptyBlocks.Enqueue(new DataBlock(5));
            }

            sut.FillQueue(stream);
            readBuffer.FilledBlocks.TryDequeue(out DataBlock b1);
            readBuffer.FilledBlocks.TryDequeue(out DataBlock b2);
            readBuffer.FilledBlocks.TryDequeue(out DataBlock b3);

            Assert.AreEqual(null, b3);
        }

        [TestMethod]
        public void BlockReader_FillQueue_SplitsDataToBlocks()
        {
            PrepareReader(out IReadBuffer readBuffer, out IReader sut);
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                readBuffer.EmptyBlocks.Enqueue(new DataBlock(5));
            }

            sut.FillQueue(stream);
            bool canDequque1 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b1);
            bool canDequque2 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b2);
            bool canDequque3 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b3);

            Assert.AreEqual(true, canDequque1);
            Assert.AreEqual(true, canDequque2);
            Assert.AreEqual(7, b2.Data[2]);
        }

        [TestMethod]
        public void BlockReader_FillQueue_CountsSequenceNumbers()
        {
            PrepareReader(out IReadBuffer readBuffer, out IReader sut);
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                readBuffer.EmptyBlocks.Enqueue(new DataBlock(1));
            }

            sut.FillQueue(stream);
            bool canDequque1 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b1);
            bool canDequque2 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b2);
            bool canDequque3 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b3);

            Assert.AreEqual(0, b1.SequenceNr);
            Assert.AreEqual(1, b2.SequenceNr);
            Assert.AreEqual(2, b3.SequenceNr);
        }

        [TestMethod]
        public void BlockReader_FillQueue_CountsBlockSize()
        {
            PrepareReader(out IReadBuffer readBuffer, out IReader sut);
            var stream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            for (int i = 0; i < 9; i++)
            {
                readBuffer.EmptyBlocks.Enqueue(new DataBlock(5));
            }

            sut.FillQueue(stream);
            bool canDequque1 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b1);
            bool canDequque2 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b2);
            bool canDequque3 = readBuffer.FilledBlocks.TryDequeue(out DataBlock b3);

            Assert.AreEqual(5, b1.Size);
            Assert.AreEqual(3, b2.Size);
        }
    }
}
