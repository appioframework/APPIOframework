using System.Collections.Generic;
using System.Linq;
using Oppo.ObjectModel.CommandStrategies.BuildCommands;
using Oppo.ObjectModel.CommandStrategies.CommandNotExistent;
using Oppo.ObjectModel.CommandStrategies.HelloCommands;
using Oppo.ObjectModel.CommandStrategies.HelpCommands;
using Oppo.ObjectModel.CommandStrategies.NewCommands;
using Oppo.ObjectModel.CommandStrategies.PublishCommands;
using Oppo.ObjectModel.CommandStrategies.VersionCommands;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandStrategyFactory : ICommandStrategyFactory
    {
        private readonly Dictionary<string, ICommandStrategy> _commands
            = new Dictionary<string, ICommandStrategy>();

        public CommandStrategyFactory(IReflection reflection, IWriter writer)
        {
            _commands.Add(Constants.CommandName.Hello, new HelloStrategy(writer));
            _commands.Add(Constants.CommandName.New, new NewStrategy(new NewCommandStrategyFactory(new FileSystemWrapper())));
            _commands.Add(Constants.CommandName.Build, new BuildStrategy(new FileSystemWrapper()));
            _commands.Add(Constants.CommandName.Publish, new PublishStrategy(new FileSystemWrapper()));
            _commands.Add(Constants.CommandName.Version, new VersionStrategy(reflection, writer));

            // help command must be added as last one, because it hold a reference to all others commands for help messages
            var helpStrategy = new HelpStrategy(writer, _commands.Values.ToList());
            _commands.Add(Constants.CommandName.Help, helpStrategy);
            _commands.Add(Constants.CommandName.ShortHelp, helpStrategy);            
        }

        public ICommandStrategy GetStrategy(string commandName)
        {           
            if (string.IsNullOrEmpty(commandName))
            {
                return _commands[Constants.CommandName.Help];
            }

            if (_commands.ContainsKey(commandName ?? string.Empty))
            {
                return _commands[commandName];
            }

            return new CommandNotExistentStrategy();
        }
    }
}