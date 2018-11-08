namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishVerboseHelpStrategy : PublishHelpStrategy
    {
        public PublishVerboseHelpStrategy(IWriter writer) : base(writer)
        {
        }

        public override string Name => Constants.PublishCommandArguments.VerboseHelp;
    }
}
