using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandStrategyFactory : ICommandStrategyFactory
    {
        private readonly Dictionary<string, ICommandStrategy> _commands
            = new Dictionary<string, ICommandStrategy>();

        public CommandStrategyFactory()
        {
            _commands.Add(Constants.CommandName.Hello, new HelloStrategy(null));
            _commands.Add(Constants.CommandName.Sln, new SlnStrategy(new SlnCommandStrategyFactory(new FileSystemWrapper())));
        }

        public ICommandStrategy GetStrategy(string commandName)
        {
            if (_commands.ContainsKey(commandName ?? string.Empty))
            {
                return _commands[commandName];
            }

            return new CommandNotExistentStrategy();
        }
    }
}