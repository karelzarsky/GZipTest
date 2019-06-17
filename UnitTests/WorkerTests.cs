using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using Ninject;

namespace UnitTests
{
    [TestClass]
    public class WorkerTests
    {
        StandardKernel kernel = new StandardKernel();

        public WorkerTests()
        {
            kernel.Load(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        public void CompressOneBlock_SequentionalArray_ReturnsLargeResult()
        {
            var sut = kernel.Get<IWorker>();

            byte[] array = Enumerable.Range(0, 255).Select(x => (byte)x).ToArray();
            var db = new DataBlock(array, 1);

            var res = sut.CompressOneBlock(db);

            Assert.IsTrue(res.Length > 256);
            Assert.IsTrue(res.Length < 300);
        }

        [TestMethod]
        public void CompressOneBlock_ArrayOfZeroes_ReturnsSmallResult()
        {
            var sut = kernel.Get<IWorker>();
            byte[] array = new byte[255];
            var db = new DataBlock(array, 1);

            var res = sut.CompressOneBlock(db);

            Assert.IsTrue(res.Length > 10);
            Assert.IsTrue(res.Length < 50);
        }

        [TestMethod]
        public void CompressAndDecompress_SequentionalArray_ReturnsOriginalData()
        {
            var sut = kernel.Get<IWorker>();
            byte[] original = Enumerable.Range(0, 255).Select(x => (byte)x).ToArray();
            var db = new DataBlock(original, 1);

            byte[] compressed = sut.CompressOneBlock(db);
            var decompressed = sut.DecompressOneBlock(new DataBlock(compressed, 1));

            Assert.AreEqual(original.Length, decompressed.Length);
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], decompressed[i]);
            }
        }

        [TestMethod]
        public void CompressAndDecompress_ArrayOfZeroes_ReturnsOriginalData()
        {
            var sut = kernel.Get<IWorker>();
            byte[] original = new byte[255];
            var db = new DataBlock(original, 1);

            byte[] compressed = sut.CompressOneBlock(db);
            var decompressed = sut.DecompressOneBlock(new DataBlock(compressed, 1));

            Assert.AreEqual(original.Length, decompressed.Length);
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], decompressed[i]);
            }
        }

        [TestMethod]
        public void CompressAndDecompress_1_ReturnsOriginalData()
        {
            var sut = kernel.Get<IWorker>();
            byte[] original = new byte[] { 1 };
            var db = new DataBlock(original, 1);

            byte[] compressed = sut.CompressOneBlock(db);
            var decompressed = sut.DecompressOneBlock(new DataBlock(compressed, 1));

            Assert.AreEqual(original.Length, decompressed.Length);
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], decompressed[i]);
            }
        }

        [TestMethod]
        public void CompressAndDecompress_Empty_ReturnsOriginalData()
        {
            var sut = kernel.Get<IWorker>();
            byte[] original = new byte[] { };
            var db = new DataBlock(original, 1);

            byte[] compressed = sut.CompressOneBlock(db);
            var decompressed = sut.DecompressOneBlock(new DataBlock(compressed, 1));

            Assert.AreEqual(original.Length, decompressed.Length);
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], decompressed[i]);
            }
        }

        [TestMethod]
        public void CompressAndDecompress_6numbers_ReturnsOriginalData()
        {
            var sut = kernel.Get<IWorker>();
            byte[] original = new byte[] { 21, 53, 11, 17, 6, 19 };
            var db = new DataBlock(original, 1);

            byte[] compressed = sut.CompressOneBlock(db);
            var decompressed = sut.DecompressOneBlock(new DataBlock(compressed, 1));

            Assert.AreEqual(original.Length, decompressed.Length);
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], decompressed[i]);
            }
        }
    }
}
