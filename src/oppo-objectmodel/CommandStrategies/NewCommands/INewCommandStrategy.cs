using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public interface INewCommandStrategy
    {
        string Execute(IEnumerable<string> inputParams);
    }
}
