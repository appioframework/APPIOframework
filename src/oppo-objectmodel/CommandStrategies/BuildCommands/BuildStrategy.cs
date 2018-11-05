using System;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildStrategy : ICommandStrategy
    {
        public string Execute(IEnumerable<string> inputsArgs)
        {
            var firstInputParam = inputsArgs.FirstOrDefault();

            if (firstInputParam == Constants.BuildCommandArguments.ModeAll)
            {
                return Constants.CommandResults.Success;
            }

            if (firstInputParam == Constants.BuildCommandArguments.ModelRebuild)
            {
                return Constants.CommandResults.Success;
            }

            return Constants.CommandResults.Failure;
        }
    }
}
