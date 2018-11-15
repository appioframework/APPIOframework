using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface IObjectModel
    {
        CommandResult ExecuteCommand(IEnumerable<string> inputParams);
    }
}