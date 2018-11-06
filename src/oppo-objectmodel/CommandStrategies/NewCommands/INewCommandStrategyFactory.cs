namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public interface INewCommandStrategyFactory
    {
        INewCommandStrategy GetStrategy(string commandName);
    }
}
