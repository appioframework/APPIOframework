namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishVerboseNameStrategy : PublishNameStrategy
    {
        public PublishVerboseNameStrategy(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Name => Constants.PublishCommandArguments.VerboseName;
    }
}
