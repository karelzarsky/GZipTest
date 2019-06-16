using System;
using System.IO;

namespace GZipTest
{
    public class ArgumentsParser : IArgumentsParser
    {
        public bool ParseArguments(string[] args, out string errorMessage, out bool compress, out Stream input, out Stream output)
        {
            errorMessage = null;
            compress = false;
            if (args.Length == 3)
            {
                try
                {
                    input = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
                    output = File.Create(args[2], 4096, FileOptions.WriteThrough);
                }
                catch (Exception)
                {
                    throw;
                }
                if (args[0].Equals("compress", StringComparison.OrdinalIgnoreCase))
                {
                    compress = true;
                    return true;
                }
                if (args[0].Equals("decompress", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            errorMessage = "Invalid arguments!\r\n" +
            "- compressing: GZipTest.exe compress [original file name] [archive file name]\r\n" +
            "- decompressing: GZipTest.exe decompress [archive file name] [decompressing file name]\r\n";
            input = output = null;
            return false;
        }
    }
}
