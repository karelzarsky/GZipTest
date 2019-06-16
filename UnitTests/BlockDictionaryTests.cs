using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;

namespace UnitTests
{
    [TestClass]
    public class BlockDictionaryTests
    {
        [TestMethod]
        public void BlockDictionary_NewInstance_IsEmpty()
        {
            var sut = new BlockDictionary(999);

            bool res = sut.Empty();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void BlockDictionary_AfterAdd_IsNotEmpty()
        {
            var sut = new BlockDictionary(999);
            sut.Add(new DataBlock(0));

            bool res = sut.Empty();

            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void BlockDictionary_AfterAdd_CanRetrive()
        {
            long nr = 456;
            var sut = new BlockDictionary(999);
            var block = new DataBlock(0)
            {
                SequenceNr = nr
            };
            sut.Add(block);

            bool success = sut.TryRetrive(nr, out DataBlock res, 100);

            Assert.AreEqual(true, success);
            Assert.AreEqual(nr, res.SequenceNr);
        }

        [TestMethod]
        public void BlockDictionary_AfterAddAndRemove_IsEmpty()
        {
            long nr = 456;
            var sut = new BlockDictionary(999);
            var block = new DataBlock(0)
            {
                SequenceNr = nr
            };
            sut.Add(block);

            bool success = sut.TryRetrive(nr, out DataBlock res, 100);
            bool empty = sut.Empty();

            Assert.AreEqual(true, empty);
        }

        [TestMethod]
        public void BlockDictionary_AfterAddTwo_CanRetriveSecond()
        {
            long nr = 456;
            var sut = new BlockDictionary(999);
            sut.Add(new DataBlock(0) { SequenceNr = nr });
            sut.Add(new DataBlock(0) { SequenceNr = nr+1 });

            bool success1 = sut.TryRetrive(nr, out DataBlock res, 100);
            bool success2 = sut.TryRetrive(nr+1, out DataBlock res2, 100);
            bool empty = sut.Empty();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(true, success2);
            Assert.AreEqual(nr, res.SequenceNr);
            Assert.AreEqual(nr+1, res2.SequenceNr);
            Assert.AreEqual(true, empty);
        }
    }
}
