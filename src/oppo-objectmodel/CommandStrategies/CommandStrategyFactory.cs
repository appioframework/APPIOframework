using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandStrategyFactory : ICommandStrategyFactory
    {
        private readonly Dictionary<string, ICommandStrategy> _commands
            = new Dictionary<string, ICommandStrategy>();

        public CommandStrategyFactory(IWriter writer)
        {
            _commands.Add(Constants.CommandName.Hello, new HelloStrategy(writer));
            _commands.Add(Constants.CommandName.Sln, new SlnStrategy(new SlnCommandStrategyFactory(new FileSystemWrapper())));
            _commands.Add(Constants.CommandName.Build, new BuildStrategy());
            _commands.Add(Constants.CommandName.Publish, new PublishStrategy());

            var helpStrategy = new HelpStrategy();
            _commands.Add(Constants.CommandName.Help, helpStrategy);
            _commands.Add(Constants.CommandName.ShortHelp, helpStrategy);
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