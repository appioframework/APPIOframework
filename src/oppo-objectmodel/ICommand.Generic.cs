using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface ICommand<TDependance>
    {
        string Name { get; }
        CommandResult Execute(IEnumerable<string> inputParams);
        string GetHelpText();
    }
}
