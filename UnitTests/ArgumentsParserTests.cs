using Microsoft.VisualStudio.TestTools.UnitTesting;
using GZipTest;
using Ninject;
using System.Reflection;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class ArgumentsParserTests
    {
        StandardKernel kernel = new StandardKernel();

        public ArgumentsParserTests()
        {
            kernel.Load(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        public void ParseArguments_NoArguments_ReturnsFalse()
        {
            var sut = kernel.Get<IArgumentsParser>();
            bool success = sut.ParseArguments(new string[0], out string error, out Stream input, out Stream output);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void ParseArguments_NoArguments_OutputsErrorMessage()
        {
            var sut = kernel.Get<IArgumentsParser>();
            bool success = sut.ParseArguments(new string[0], out string error, out Stream input, out Stream output);
            Assert.IsFalse(string.IsNullOrEmpty(error));
        }

        [TestMethod]
        public void ParseArguments_UnknownWord_ReturnsFalse()
        {
            var sut = kernel.Get<IArgumentsParser>();
            bool success = sut.ParseArguments(new string[] {"xxx", "", "" }, out string error, out Stream input, out Stream output);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void ParseArguments_UnknownWord_OutputsErrorMessage()
        {
            var sut = kernel.Get<IArgumentsParser>();
            bool success = sut.ParseArguments(new string[] { "xxx", "", "" }, out string error, out Stream input, out Stream output);
            Assert.IsFalse(string.IsNullOrEmpty(error));
        }
    }
}
