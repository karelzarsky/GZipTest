﻿using Ninject.Modules;

namespace GZipTest
{
    public class Bindings : NinjectModule
    {
        /// <summary>
        /// Binding for dependency injection container
        /// </summary>
        public override void Load()
        {
            Bind<IBlockDictionary>().To<BlockDictionary>().InSingletonScope();
            Bind<IBlockQueue>().To<BlockQueue>();
            Bind<IReader>().To<Reader>();
            Bind<IWriter>().To<Writer>();
            Bind<IDataBlock>().To<DataBlock>();
            Bind<IWorker>().To<Worker>();
            Bind<IStatistics>().To<Statistics>().InSingletonScope();
            Bind<IThreadsCreator>().To<ThreadsCreator>();
            Bind<IArgumentsParser>().To<ArgumentsParser>();
            Bind<ISettings>().To<Settings>().InSingletonScope();
            Bind<IReadBuffer>().To<ReadBuffer>().InSingletonScope();
        }
    }
}
