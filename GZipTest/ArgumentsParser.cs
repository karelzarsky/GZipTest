using System;
using System.IO;

namespace GZipTest
{
    public class ArgumentsParser : IArgumentsParser
    {
        public bool ParseArguments(string[] args, out string msg, out bool compress, out Stream input, out Stream output)
        {
            msg = null;
            input = output = null;
            compress = false;
            if (args.Length != 3 || (!args[0].Equals("compress",   StringComparison.OrdinalIgnoreCase) && 
                                      args[0].Equals("decompress", StringComparison.OrdinalIgnoreCase)))
            {
                msg = "Invalid arguments!\r\n" +
                "- compressing: GZipTest.exe compress [original file name] [archive file name]\r\n" +
                "- decompressing: GZipTest.exe decompress [archive file name] [decompressing file name]\r\n";
                input = output = null;
                return false;
            }
            if (args[0].Equals("compress", StringComparison.OrdinalIgnoreCase))
            {
                compress = true;
            }
            try
            {
                input = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            }
            catch (DirectoryNotFoundException) { msg = "Input directory not found."; return false; }
            catch (PathTooLongException) { msg = "Input path too long."; return false; }
            catch (FileNotFoundException) { msg = "Input file not found."; return false; }
            catch (IOException) { msg = "Cannot open input file."; return false; }
            catch (System.Security.SecurityException) { msg = "Insufficient rights to open input file."; return false; }
            catch (UnauthorizedAccessException) { msg = "Unauthorized access for input file."; return false; }
            catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException || ex is NotSupportedException) { msg = "Invalid input path."; return false; }
            try
            {
                output = File.Create(args[2], 4096, FileOptions.WriteThrough);
            }
            catch (DirectoryNotFoundException) { msg = "Output directory not found."; return false; }
            catch (PathTooLongException) { msg = "Output path too long."; return false; }
            catch (FileNotFoundException) { msg = "Output file not found."; return false; }
            catch (IOException) { msg = "Cannot open output file."; return false; }
            catch (System.Security.SecurityException) { msg = "Insufficient rights to open output file."; return false; }
            catch (UnauthorizedAccessException) { msg = "Unauthorized access for output file."; return false; }
            catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException || ex is NotSupportedException) { msg = "Invalid output path."; return false; }
            return true;
        }
    }
}
