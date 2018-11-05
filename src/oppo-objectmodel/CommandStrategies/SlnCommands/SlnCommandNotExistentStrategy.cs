using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnCommandNotExistentStrategy : ISlnCommandStrategy
    {
        public string Execute(IEnumerable<string> inputParams)
        {
            return Constants.CommandResults.Failure;
        }
    }
}