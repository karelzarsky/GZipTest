using System.IO;

namespace GZipTest
{
    public interface IArgumentsParser
    {
        bool ParseArguments(string[] args, out string errorMessage, out bool compress, out Stream input, out Stream output);
    }
}