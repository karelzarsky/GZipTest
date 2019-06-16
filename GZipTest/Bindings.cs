using Ninject.Modules;

namespace GZipTest
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IBlockDictionary>().To<BlockDictionary>();
            Bind<IBlockQueue>().To<BlockQueue>();
            Bind<IBlockReader>().To<BlockReader>();
            Bind<IBlockWriter>().To<BlockWriter>();
            Bind<IDataBlock>().To<DataBlock>();
            Bind<IGZipWorker>().To<GZipWorker>();
            Bind<IStatistics>().To<Statistics>();
            Bind<IThreadsCreator>().To<ThreadsCreator>();
            Bind<IArgumentsParser>().To<ArgumentsParser>();
            
        }
    }
}
