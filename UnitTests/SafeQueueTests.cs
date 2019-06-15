using System;
using GZipTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SafeQueueTests
    {
        [TestMethod]
        public void SafeQueue_TryDequeueOnEmpty_ReturnsFalse()
        {
            var sut = new SafeQueue();

            Assert.AreEqual(false, sut.TryDequeue(out DataBlock b, 10));
        }

        [TestMethod]
        public void SafeQueue_SuccessfullDequeue_ReturnsTrue()
        {
            var sut = new SafeQueue();

            sut.Enqueue(new DataBlock(0));

            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b, 10));
            Assert.AreNotEqual(null, b);
        }

        [TestMethod]
        public void SafeQueue_AfterDequeueAll_NothingRemains()
        {
            var sut = new SafeQueue();

            sut.Enqueue(new DataBlock(0));
            sut.Enqueue(new DataBlock(0));
            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b1, 10));
            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b2, 10));

            Assert.AreEqual(false, sut.TryDequeue(out DataBlock b3, 10));
        }
    }
}
