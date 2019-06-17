﻿using System.IO;

namespace GZipTest
{
    public interface IBlockReader
    {
        void FillQueue(Stream stream, IBlockQueue queueEmpty, IBlockQueue queueFilled, ref long totalBlocks);
    }
}