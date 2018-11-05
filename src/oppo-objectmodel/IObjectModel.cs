using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface IObjectModel
    {
        string ExecuteCommand(IEnumerable<string> inputParams);
    }
}