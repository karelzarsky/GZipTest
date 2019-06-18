using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    public class ArgumentsParser : IArgumentsParser
    {
        private readonly ISettings settings;

        public ArgumentsParser(ISettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Parse commandline arguments
        /// </summary>
        /// <param name="args">arguments</param>
        /// <param name="msg">error message</param>
        /// <param name="input">stream to read from</param>
        /// <param name="output">stream to write to</param>
        /// <returns>True when all arguments are correct and files were successfully opened.</returns>
        public bool ParseArguments(string[] args, out string msg, out Stream input, out Stream output)
        {
            msg = null;
            input = output = null;
            if (args.Length != 3 ||
                (!args[0].Equals("compress", StringComparison.OrdinalIgnoreCase) &&
                 !args[0].Equals("decompress", StringComparison.OrdinalIgnoreCase)))
            {
                msg = "Invalid arguments!\r\n" +
                "- compressing: GZipTest.exe compress [original file name] [archive file name]\r\n" +
                "- decompressing: GZipTest.exe decompress [archive file name] [decompressing file name]\r\n";
                input = output = null;
                return false;
            }
            if (args[0].Equals("compress", StringComparison.OrdinalIgnoreCase))
            {
                settings.Mode = CompressionMode.Compress;
            }
            return OpenSourceFile(args, out msg, out input) && OpenDestinationFile(args, out msg, out output);
        }

        /// <summary>
        /// Open file stream for readig data
        /// </summary>
        private bool OpenSourceFile(string[] args, out string msg, out Stream input)
        {
            msg = null;
            input = null;
            try
            {
                input = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
                return true;
            }
            catch (DirectoryNotFoundException) { msg = "Input directory not found."; return false; }
            catch (PathTooLongException) { msg = "Input path too long."; return false; }
            catch (FileNotFoundException) { msg = "Input file not found."; return false; }
            catch (IOException) { msg = "Cannot open input file."; return false; }
            catch (System.Security.SecurityException) { msg = "Insufficient rights to open input file."; return false; }
            catch (UnauthorizedAccessException) { msg = "Unauthorized access for input file."; return false; }
            catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException || ex is NotSupportedException) { msg = "Invalid input path."; return false; }
        }

        /// <summary>
        /// Open file stream for writing data
        /// </summary>
        private bool OpenDestinationFile(string[] args, out string msg, out Stream output)
        {
            msg = null;
            output = null;
            try
            {
                output = File.Create(args[2], 4096, FileOptions.WriteThrough);
                return true;
            }
            catch (DirectoryNotFoundException) { msg = "Output directory not found."; return false; }
            catch (PathTooLongException) { msg = "Output path too long."; return false; }
            catch (FileNotFoundException) { msg = "Output file not found."; return false; }
            catch (IOException) { msg = "Cannot open output file."; return false; }
            catch (System.Security.SecurityException) { msg = "Insufficient rights to open output file."; return false; }
            catch (UnauthorizedAccessException) { msg = "Unauthorized access for output file."; return false; }
            catch (Exception ex) when(ex is ArgumentException || ex is ArgumentNullException || ex is NotSupportedException) { msg = "Invalid output path."; return false; }
        }
    }
}
