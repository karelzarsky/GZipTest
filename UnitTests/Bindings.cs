using Ninject.Modules;

namespace GZipTest
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IBlockDictionary>().To<BlockDictionary>();
            Bind<IBlockQueue>().To<BlockQueue>();
            Bind<IReader>().To<Reader>();
            Bind<IWriter>().To<Writer>();
            Bind<IDataBlock>().To<DataBlock>();
            Bind<IWorker>().To<Worker>();
            Bind<IStatistics>().To<Statistics>();
            Bind<IThreadsCreator>().To<ThreadsCreator>();
            Bind<IArgumentsParser>().To<ArgumentsParser>();
            Bind<ISettings>().To<Settings>();
            Bind<IReadBuffer>().To<ReadBuffer>().InSingletonScope();
        }
    }
}
