using System;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandNotExistentStrategy
        : ICommandStrategy
    {
        public string Name => throw new NotSupportedException();

        public string Execute(IEnumerable<string> inputsParams)
        {
            return Constants.CommandResults.Failure;
        }

        public string GetHelpText()
        {
            throw new NotSupportedException();
        }
    }
}