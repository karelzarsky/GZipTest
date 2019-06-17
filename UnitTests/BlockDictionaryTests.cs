using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using Ninject;

namespace UnitTests
{
    [TestClass]
    public class BlockDictionaryTests
    {
        StandardKernel kernel = new StandardKernel();

        [TestMethod]
        public void BlockDictionary_NewInstance_IsEmpty()
        {
            var sut = kernel.Get<IBlockDictionary>();

            bool res = sut.Empty();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void BlockDictionary_AfterAdd_IsNotEmpty()
        {
            var sut = kernel.Get<IBlockDictionary>();
            sut.Add(new DataBlock(0));

            bool res = sut.Empty();

            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void BlockDictionary_AfterAdd_CanRetrive()
        {
            long nr = 456;
            var sut = kernel.Get<IBlockDictionary>();
            var block = new DataBlock(0)
            {
                SequenceNr = nr
            };
            sut.Add(block);

            bool success = sut.TryRetrive(nr, out DataBlock res);

            Assert.AreEqual(true, success);
            Assert.AreEqual(nr, res.SequenceNr);
        }

        [TestMethod]
        public void BlockDictionary_AfterAddAndRemove_IsEmpty()
        {
            long nr = 456;
            var sut = kernel.Get<IBlockDictionary>();
            var block = new DataBlock(0)
            {
                SequenceNr = nr
            };
            sut.Add(block);

            bool success = sut.TryRetrive(nr, out DataBlock res);
            bool empty = sut.Empty();

            Assert.AreEqual(true, empty);
        }

        [TestMethod]
        public void BlockDictionary_AfterAddTwo_CanRetriveSecond()
        {
            long nr = 456;
            var sut = kernel.Get<IBlockDictionary>();
            sut.Add(new DataBlock(0) { SequenceNr = nr });
            sut.Add(new DataBlock(0) { SequenceNr = nr+1 });

            bool success1 = sut.TryRetrive(nr, out DataBlock res);
            bool success2 = sut.TryRetrive(nr+1, out DataBlock res2);
            bool empty = sut.Empty();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(true, success2);
            Assert.AreEqual(nr, res.SequenceNr);
            Assert.AreEqual(nr+1, res2.SequenceNr);
            Assert.AreEqual(true, empty);
        }
    }
}
