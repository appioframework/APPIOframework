using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public interface IBuildStrategy
    {
        string Execute(IEnumerable<string> inputsParams);
    }
}
