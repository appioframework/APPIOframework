using System.Collections.Generic;

namespace Appio.ObjectModel
{
    public interface ICommand<TDependance> : ICommand
	{
	}

	public interface ICommand
	{
		string Name { get; }
		CommandResult Execute(IEnumerable<string> inputParams);
		string GetHelpText();
	}
}
