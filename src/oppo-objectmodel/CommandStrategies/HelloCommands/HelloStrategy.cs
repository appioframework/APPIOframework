using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.HelloCommands
{
    public class HelloStrategy : ICommand<ObjectModel>
    {
        private readonly IWriter _writer;

        public HelloStrategy(IWriter writer)
        {
            _writer = writer;
        }

        public string Name => Constants.CommandName.Hello;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            _writer.WriteLine(Constants.HelloString);
            return new CommandResult(true, string.Empty);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.HelloCommand;
        }
    }
}
