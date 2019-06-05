using System.Collections.Generic;

namespace Appio.ObjectModel.CommandStrategies.HelloCommands
{
    public class HelloStrategy : ICommand<ObjectModel>
    {
        public string Name => Constants.CommandName.Hello;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();
            outputMessages.Add(Constants.HelloString, string.Empty);            
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.HelloCommand;
        }
    }
}