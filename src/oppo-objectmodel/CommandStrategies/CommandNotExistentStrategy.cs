using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandNotExistentStrategy
        : ICommandStrategy
    {
        public string Execute(IEnumerable<string> inputsParams)
        {
            return Constants.CommandResults.Failure;
        }
    }
}