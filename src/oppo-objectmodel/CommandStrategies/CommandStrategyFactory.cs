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
            var fileSystem = new FileSystemWrapper();

            _commands.Add(Constants.CommandName.Hello, new HelloStrategy(writer));

            var newStrategies = new ICommand<NewStrategy>[] { new NewSlnCommandStrategy(fileSystem), new NewOpcuaAppCommandStrategy(fileSystem), new NewHelpCommandStrategy(writer), new NewVerboseHelpCommandStrategy(writer), };
            var newStrategiesCommandFactory = new CommandFactory<NewStrategy>(newStrategies, Constants.NewCommandName.Help);
            _commands.Add(Constants.CommandName.New, new NewStrategy(newStrategiesCommandFactory));

            _commands.Add(Constants.CommandName.Build, new BuildStrategy(new BuildCommandStrategyFactory(writer, new FileSystemWrapper())));

            var publishStrategies = new ICommand<PublishStrategy>[] { new PublishNameStrategy(fileSystem), new PublishVerboseNameStrategy(fileSystem), new PublishHelpStrategy(writer), new PublishVerboseHelpStrategy(writer), };
            var publishStrategyCommandFactory = new CommandFactory<PublishStrategy>(publishStrategies, Constants.PublishCommandArguments.Help);
            _commands.Add(Constants.CommandName.Publish, new PublishStrategy(publishStrategyCommandFactory));

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

            if (_commands.ContainsKey(commandName))
            {
                return _commands[commandName];
            }

            return new CommandNotExistentStrategy();
        }
    }
}