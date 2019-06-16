﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;

namespace UnitTests
{
    [TestClass]
    public class GZipWorkerTests
    {
        [TestMethod]
        public void CompressOneBlock_SequentionalArray_ReturnsLargeResult()
        {
            var sut = new GZipWorker();
            byte[] array = Enumerable.Range(0, 255).Select(x => (byte)x).ToArray();
            var db = new DataBlock(array, 1);

            var res = sut.CompressOneBlock(db);

            Assert.IsTrue(res.Length > 256);
            Assert.IsTrue(res.Length < 300);
        }

        [TestMethod]
        public void CompressOneBlock_ArrayOfZeroes_ReturnsSmallResult()
        {
            var sut = new GZipWorker();
            byte[] array = new byte[255];
            var db = new DataBlock(array, 1);

            var res = sut.CompressOneBlock(db);

            Assert.IsTrue(res.Length > 10);
            Assert.IsTrue(res.Length < 50);
        }
    }
}
