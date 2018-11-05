using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandStrategyFactory : ICommandStrategyFactory
    {
        private readonly Dictionary<string, ICommandStrategy> commands
            = new Dictionary<string, ICommandStrategy>();

        public CommandStrategyFactory()
        {
            commands.Add(Constants.CommandName.Sln, new SlnStrategy(new SlnCommandStrategyFactory(new FileSystemWrapper())));
        }

        public ICommandStrategy GetStrategy(string commandName)
        {
            if (commands.ContainsKey(commandName ?? string.Empty))
            {
                return commands[commandName];
            }

            return new CommandNotExistentStrategy();
        }
    }
}