using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands    
{
    public interface ISlnCommandStrategy
    {
        string Execute(IEnumerable<string> inputParams);
    }
}