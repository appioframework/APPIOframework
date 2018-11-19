using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.HelpCommands
{
    public class HelpStrategy : ICommand<ObjectModel>
    {
        public HelpStrategy(string helpCommandName)
        {
            Name = helpCommandName;
        }

        public ICommandFactory<ObjectModel> CommandFactory { get; set; }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines();
            outputMessages.Add(Resources.text.help.HelpTextValues.HelpStartCommand, string.Empty);
            
            foreach (var command in CommandFactory?.Commands ?? new ICommand<ObjectModel>[0])
            {                
                outputMessages.Add(command.Name, command.GetHelpText());
            }

            outputMessages.Add(string.Empty, Resources.text.help.HelpTextValues.HelpEndCommand);
            
            OppoLogger.Info(LoggingText.OppoHelpCalled);
            return new CommandResult(true, outputMessages);            
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.HelpCommand;
        }
    }
}