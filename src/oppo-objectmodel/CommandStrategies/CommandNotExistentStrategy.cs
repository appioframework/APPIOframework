using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class CommandNotExistentStrategy
        : ICommandStrategy
    {
        [ExcludeFromCodeCoverage]
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