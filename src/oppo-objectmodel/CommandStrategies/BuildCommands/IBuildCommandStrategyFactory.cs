namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public interface IBuildCommandStrategyFactory
    {
        IBuildStrategy GetStrategy(string commandName);
    }
}