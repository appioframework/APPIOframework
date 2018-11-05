using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandNotExistentStrategy
        : ICommandStrategy
    {
        public string Execute(IEnumerable<string> inputsArgs)
        {
            return Constants.CommandResults.Failure;
        }
    }
}