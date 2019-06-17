using Ninject.Modules;

namespace GZipTest
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IBlockDictionary>().To<BlockDictionary>().InSingletonScope();
            Bind<IBlockQueue>().To<BlockQueue>();
            Bind<IBlockReader>().To<BlockReader>();
            Bind<IBlockWriter>().To<BlockWriter>();
            Bind<IDataBlock>().To<DataBlock>();
            Bind<IWorker>().To<Worker>();
            Bind<IStatistics>().To<Statistics>().InSingletonScope();
            Bind<IThreadsCreator>().To<ThreadsCreator>();
            Bind<IArgumentsParser>().To<ArgumentsParser>();
            Bind<ISettings>().To<Settings>().InSingletonScope();
        }
    }
}
