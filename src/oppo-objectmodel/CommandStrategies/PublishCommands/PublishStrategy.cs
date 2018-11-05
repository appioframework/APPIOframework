using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishStrategy : ICommandStrategy
    {
        public string Execute(IEnumerable<string> inputsArgs)
        {
            var firstInputParam = inputsArgs.FirstOrDefault();

            if (firstInputParam == Constants.PublishCommandArguments.ModeAll)
            {
                // command logic here..

                return Constants.CommandResults.Success;
            }

            return Constants.CommandResults.Failure;
        }
    }
}
