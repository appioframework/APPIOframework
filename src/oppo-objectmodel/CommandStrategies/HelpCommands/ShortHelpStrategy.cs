namespace Oppo.ObjectModel.CommandStrategies.HelpCommands
{
    public class ShortHelpStrategy : HelpStrategy
    {
        public ShortHelpStrategy(IWriter writer)
            : base(writer)
        {
        }

        public override string Name => Constants.CommandName.ShortHelp;
    }
}
