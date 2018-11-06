using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class HelpStrategy : ICommandStrategy
    {
        public string Execute(IEnumerable<string> inputsParams)
        {
            // command logic here..
            return Constants.CommandResults.Success;            
        }
    }
}