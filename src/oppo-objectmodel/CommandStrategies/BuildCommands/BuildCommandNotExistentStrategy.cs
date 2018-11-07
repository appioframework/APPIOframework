using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildCommandNotExistentStrategy : IBuildStrategy
    {
        public string Execute(IEnumerable<string> inputsParams)
        {
            return Constants.CommandResults.Failure;
        }
    }
}