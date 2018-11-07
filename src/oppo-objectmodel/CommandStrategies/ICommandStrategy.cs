using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.ObjectModel.CommandStrategies
{
    public interface ICommandStrategy
    {
        [ExcludeFromCodeCoverage]
        string Name { get; }

        string Execute(IEnumerable<string> inputsParams);
        string GetHelpText();
    }
}