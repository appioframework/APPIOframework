using System;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildStrategy : ICommandStrategy
    {
        public string Execute(IEnumerable<string> inputsParams)
        {
            var firstInputParam = inputsParams.FirstOrDefault();

            if (firstInputParam == Constants.BuildCommandArguments.ModeAll)
            {
                // command logic here..

                return Constants.CommandResults.Success;
            }

            if (firstInputParam == Constants.BuildCommandArguments.ModelRebuild)
            {
                // command logic here..

                return Constants.CommandResults.Success;
            }

            return Constants.CommandResults.Failure;
        }
    }
}
