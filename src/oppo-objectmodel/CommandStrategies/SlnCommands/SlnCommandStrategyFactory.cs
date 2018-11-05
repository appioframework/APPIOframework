using System.Collections.Generic;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;

namespace Oppo.ObjectModel
{
    public class SlnCommandStrategyFactory : ISlnCommandStrategyFactory
    {
        private readonly IFileSystem _fileSystemWrapper;

        private readonly Dictionary<string, ISlnCommandStrategy> slnCommands
            = new Dictionary<string, ISlnCommandStrategy>();

        public SlnCommandStrategyFactory(IFileSystem fileSystemWrapper)
        {
            _fileSystemWrapper = fileSystemWrapper;
            slnCommands.Add(Constants.SlnCommandName.New, new SlnNewCommandStrategy(fileSystemWrapper));
        }

        public ISlnCommandStrategy GetStrategy(string slnCommandName)
        {
            if (slnCommands.ContainsKey(slnCommandName ?? string.Empty))
            {
                return slnCommands[slnCommandName];
            }

            return new SlnCommandNotExistentStrategy();
        }
    }
}