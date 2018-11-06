using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewCommandNotExistentStrategy : INewCommandStrategy
    {
        public string Execute(IEnumerable<string> inputParams)
        {
            return Constants.CommandResults.Failure;
        }
    }
}
