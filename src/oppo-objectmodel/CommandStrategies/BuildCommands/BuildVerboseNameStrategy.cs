namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildVerboseNameStrategy : BuildNameStrategy
    {
        public BuildVerboseNameStrategy(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Name => Constants.BuildCommandArguments.VerboseName;
    }
}
