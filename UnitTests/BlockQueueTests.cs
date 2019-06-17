using GZipTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace UnitTests
{
    [TestClass]
    public class BlockQueueTests
    {
        StandardKernel kernel = new StandardKernel();

        [TestMethod]
        public void BlockQueue_TryDequeueOnEmpty_ReturnsFalse()
        {
            var sut = kernel.Get<IBlockQueue>();

            Assert.AreEqual(false, sut.TryDequeue(out DataBlock b));
        }

        [TestMethod]
        public void BlockQueue_SuccessfullDequeue_ReturnsTrue()
        {
            var sut = kernel.Get<IBlockQueue>();

            sut.Enqueue(new DataBlock(0));

            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b));
            Assert.AreNotEqual(null, b);
        }

        [TestMethod]
        public void BlockQueue_AfterDequeueAll_NothingRemains()
        {
            var sut = kernel.Get<IBlockQueue>();

            sut.Enqueue(new DataBlock(0));
            sut.Enqueue(new DataBlock(0));
            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b1));
            Assert.AreEqual(true, sut.TryDequeue(out DataBlock b2));
            bool res = sut.Empty();

            Assert.AreEqual(true, res);
            Assert.AreEqual(false, sut.TryDequeue(out DataBlock b3));
        }

        [TestMethod]
        public void BlockQueue_NewInstance_IsEmpty()
        {
            var sut = kernel.Get<IBlockQueue>();

            bool res = sut.Empty();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void BlockQueue_AfterEnqueue_IsNotEmpty()
        {
            var sut = kernel.Get<IBlockQueue>();
            sut.Enqueue(new DataBlock(0));

            bool res = sut.Empty();

            Assert.AreEqual(false, res);
        }
    }
}
