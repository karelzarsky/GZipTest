using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using Ninject;

namespace UnitTests
{
    [TestClass]
    public class BlockDictionaryTests
    {
        StandardKernel kernel = new StandardKernel();
        public BlockDictionaryTests()
        {
            kernel.Load(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        public void BlockDictionary_NewInstance_IsEmpty()
        {
            var sut = kernel.Get<IBlockDictionary>();

            bool res = sut.IsEmpty();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void BlockDictionary_AfterAdd_IsNotEmpty()
        {
            var sut = kernel.Get<IBlockDictionary>();
            sut.Add(new DataBlock(0));

            bool res = sut.IsEmpty();

            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void BlockDictionary_AfterAdd_CanRetrieve()
        {
            long nr = 3;
            var sut = kernel.Get<IBlockDictionary>();
            var block = new DataBlock(0)
            {
                SequenceNr = nr
            };
            sut.Add(block);

            bool success = sut.TryRetrieve(nr, out DataBlock res);

            Assert.AreEqual(true, success);
            Assert.AreEqual(nr, res.SequenceNr);
        }

        [TestMethod]
        public void BlockDictionary_AfterAddAndRetrieve_IsEmpty()
        {
            long nr = 3;
            var sut = kernel.Get<IBlockDictionary>();
            var block = new DataBlock(0)
            {
                SequenceNr = nr
            };
            sut.Add(block);

            bool success = sut.TryRetrieve(nr, out DataBlock res);
            bool empty = sut.IsEmpty();

            Assert.AreEqual(true, empty);
        }

        [TestMethod]
        public void BlockDictionary_AfterAddTwo_CanRetrieveSecond()
        {
            long nr = 3;
            var sut = kernel.Get<IBlockDictionary>();
            sut.Add(new DataBlock(0) { SequenceNr = nr });
            sut.Add(new DataBlock(0) { SequenceNr = nr+1 });

            bool success1 = sut.TryRetrieve(nr, out DataBlock res);
            bool success2 = sut.TryRetrieve(nr+1, out DataBlock res2);
            bool empty = sut.IsEmpty();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(true, success2);
            Assert.AreEqual(nr, res.SequenceNr);
            Assert.AreEqual(nr+1, res2.SequenceNr);
            Assert.AreEqual(true, empty);
        }
    }
}
