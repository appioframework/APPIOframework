using Oppo.ObjectModel.CommandStrategies.SlnCommands;

namespace Oppo.ObjectModel
{
    public interface ISlnCommandStrategyFactory
    {       
        ISlnCommandStrategy GetStrategy( string slnCommandName );
    }
}