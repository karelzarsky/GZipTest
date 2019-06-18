using GZipTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace UnitTests
{
    [TestClass]
    public class BlockQueueTests
    {
        StandardKernel kernel = new StandardKernel();

        public BlockQueueTests()
        {
            kernel.Load(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        public void BlockQueue_SuccessfullDequeue_ReturnsTrue()
        {
            var sut = kernel.Get<IBlockQueue>();

            sut.Enqueue(new DataBlock(0));

            Assert.AreEqual(true, sut.Dequeue(out DataBlock b));
            Assert.AreNotEqual(null, b);
        }

        [TestMethod]
        public void BlockQueue_AfterDequeueAll_NothingRemains()
        {
            var sut = kernel.Get<IBlockQueue>();

            sut.Enqueue(new DataBlock(0));
            sut.Enqueue(new DataBlock(0));
            Assert.AreEqual(true, sut.Dequeue(out DataBlock b1));
            Assert.AreEqual(true, sut.Dequeue(out DataBlock b2));
            bool res = sut.IsEmpty();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void BlockQueue_NewInstance_IsEmpty()
        {
            var sut = kernel.Get<IBlockQueue>();

            bool res = sut.IsEmpty();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void BlockQueue_AfterEnqueue_IsNotEmpty()
        {
            var sut = kernel.Get<IBlockQueue>();
            sut.Enqueue(new DataBlock(0));

            bool res = sut.IsEmpty();

            Assert.AreEqual(false, res);
        }
    }
}
