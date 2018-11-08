namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewVerboseHelpCommandStrategy : NewHelpCommandStrategy
    {
        public NewVerboseHelpCommandStrategy(IWriter writer)
            : base(writer)
        {
        }

        public override string Name => Constants.NewCommandName.VerboseHelp;
    }
}
