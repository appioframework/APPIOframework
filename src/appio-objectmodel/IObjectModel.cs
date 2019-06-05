using System.Collections.Generic;

namespace Appio.ObjectModel
{
    public interface IObjectModel
    {
        CommandResult ExecuteCommand(IEnumerable<string> inputParams);
    }
}