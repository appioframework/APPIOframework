using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface ICommandFactory<TDependance>
    {
        IEnumerable<ICommand<TDependance>> Commands { get; }
        ICommand<TDependance> GetCommand(string commandName);
    }
}
