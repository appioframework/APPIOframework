namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildVerboseHelpStrategy : BuildHelpStrategy
    {
        public BuildVerboseHelpStrategy(IWriter writer)
            : base(writer)
        {
        }

        public override string Name => Constants.BuildCommandArguments.VerboseHelp;
    }
}
