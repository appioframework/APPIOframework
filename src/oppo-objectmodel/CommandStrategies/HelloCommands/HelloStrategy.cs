using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.HelloCommands
{
    public class HelloStrategy : ICommand<ObjectModel>
    {
        public string Name => Constants.CommandName.Hello;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessage = new Dictionary<string, string>();
            outputMessage.Add(Constants.HelloString, string.Empty);            
            return new CommandResult(true, string.Empty, outputMessage);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.HelloCommand;
        }
    }
}
