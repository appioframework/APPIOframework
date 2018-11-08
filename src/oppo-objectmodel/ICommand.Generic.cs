using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface ICommand<TDependance>
    {
        string Name { get; }
        string Execute(IEnumerable<string> inputsParams);
        string GetHelpText();
    }
}
