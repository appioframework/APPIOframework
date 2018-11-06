using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public interface ICommandStrategy
    {
        string Execute(IEnumerable<string> inputsParams);
    }
}