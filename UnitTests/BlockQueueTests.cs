using GZipTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class BlockQueueTests
    {
        [TestMethod]
        public void BlockQueue_TryDequeueOnEmpty_ReturnsFalse()
        {
            var sut = new BlockQueue();

            Assert.AreEqual(false, sut.TryDequeue(out DataBlock b, 10));
        }

        [TestMethod]
        public void BlockQueue_SuccessfullDequeue_ReturnsTrue()
        {
            var sut = new BlockQueue();

            sut.Enqueue(new DataBlock(0));

            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b, 10));
            Assert.AreNotEqual(null, b);
        }

        [TestMethod]
        public void BlockQueue_AfterDequeueAll_NothingRemains()
        {
            var sut = new BlockQueue();

            sut.Enqueue(new DataBlock(0));
            sut.Enqueue(new DataBlock(0));
            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b1, 10));
            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b2, 10));
            bool res = sut.Empty();

            Assert.AreEqual(true, res);
            Assert.AreEqual(false, sut.TryDequeue(out DataBlock b3, 10));
        }

        [TestMethod]
        public void BlockQueue_NewInstance_IsEmpty()
        {
            var sut = new BlockQueue();

            bool res = sut.Empty();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void BlockQueue_AfterEnqueue_IsNotEmpty()
        {
            var sut = new BlockQueue();
            sut.Enqueue(new DataBlock(0));

            bool res = sut.Empty();

            Assert.AreEqual(false, res);
        }
    }
}
