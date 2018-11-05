using Oppo.ObjectModel.CommandStrategies;

namespace Oppo.ObjectModel
{
    public interface ICommandStrategyFactory
    {       
        ICommandStrategy GetStrategy(string commandName);
    }
}