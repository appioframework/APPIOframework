using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewCommandStrategyFactory : INewCommandStrategyFactory
    {
        private readonly Dictionary<string, INewCommandStrategy> _commands = new Dictionary<string, INewCommandStrategy>();

        public NewCommandStrategyFactory(IFileSystem fileSystem)
        {
            _commands.Add(Constants.NewCommandName.Sln, new NewSlnCommandStrategy(fileSystem));
            _commands.Add(Constants.NewCommandName.OpcuaApp, new NewOpcuaAppCommandStrategy(fileSystem));
        }

        public INewCommandStrategy GetStrategy(string commandName)
        {
            if (_commands.ContainsKey(commandName ?? string.Empty))
            {
                return _commands[commandName];
            }

            return new NewCommandNotExistentStrategy();
        }
    }
}
