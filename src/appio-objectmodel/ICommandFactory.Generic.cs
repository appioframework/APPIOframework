using System.Collections.Generic;

namespace Appio.ObjectModel
{
    public interface ICommandFactory<TDependance>
    {
        IEnumerable<ICommand<TDependance>> Commands { get; }
        ICommand<TDependance> GetCommand(string commandName);
    }
}
