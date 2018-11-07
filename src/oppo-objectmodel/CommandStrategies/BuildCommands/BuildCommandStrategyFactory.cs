using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildCommandStrategyFactory : IBuildCommandStrategyFactory
    {
        private readonly Dictionary<string, IBuildStrategy> _commands = new Dictionary<string, IBuildStrategy>();

        public BuildCommandStrategyFactory(IWriter writer, IFileSystem fileSystem)
        {
            _commands.Add(Constants.BuildCommandArguments.Help, new BuildHelpStrategy(writer));
            _commands.Add(Constants.BuildCommandArguments.VerboseHelp, new BuildHelpStrategy(writer));
            _commands.Add(Constants.BuildCommandArguments.Name, new BuildNameStrategy(fileSystem));
            _commands.Add(Constants.BuildCommandArguments.VerboseName, new BuildNameStrategy(fileSystem));
        }

        public IBuildStrategy GetStrategy(string commandName)
        {
            if (_commands.ContainsKey(commandName ?? string.Empty))
            {
                return _commands[commandName];
            }

            return new BuildCommandNotExistentStrategy();
        }
    }
}
