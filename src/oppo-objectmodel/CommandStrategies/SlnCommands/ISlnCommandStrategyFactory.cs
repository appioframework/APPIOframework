namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public interface ISlnCommandStrategyFactory
    {       
        ISlnCommandStrategy GetStrategy(string slnCommandName);
    }
}