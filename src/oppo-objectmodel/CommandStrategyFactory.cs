using Oppo.ObjectModel.CommandStrategies;

namespace Oppo.ObjectModel
{
    public class CommandStrategyFactory
    {
        public ICommandStrategy GetStrategy( string commandName )
        {
            return new CommandNotExistentStrategy();
        }
    }
}