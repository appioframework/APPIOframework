using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public interface ICommandStrategy
    {
        string Name { get; }

        string Execute(IEnumerable<string> inputsParams);
        string GetHelpText();
    }
}