using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnCommandStrategyFactory : ISlnCommandStrategyFactory
    {
        private readonly Dictionary<string, ISlnCommandStrategy> _slnCommands
            = new Dictionary<string, ISlnCommandStrategy>();

        public SlnCommandStrategyFactory(IFileSystem fileSystemWrapper)
        {
            _slnCommands.Add(Constants.SlnCommandName.New, new SlnNewCommandStrategy(fileSystemWrapper));
        }

        public ISlnCommandStrategy GetStrategy(string slnCommandName)
        {
            if (_slnCommands.ContainsKey(slnCommandName ?? string.Empty))
            {
                return _slnCommands[slnCommandName];
            }

            return new SlnCommandNotExistentStrategy();
        }
    }
}