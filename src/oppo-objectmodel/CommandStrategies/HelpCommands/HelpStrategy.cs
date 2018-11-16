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
            var helpOutput = new Dictionary<string, string>();
            
            helpOutput.Add(Resources.text.help.HelpTextValues.HelpStartCommand, string.Empty);

            foreach (var command in CommandFactory?.Commands ?? new ICommand<ObjectModel>[0])
            {                
                helpOutput.Add(command.Name, command.GetHelpText());
            }

            helpOutput.Add(Resources.text.help.HelpTextValues.HelpEndCommand, string.Empty);
            OppoLogger.Info(LoggingText.OppoHelpCalled);
            return new CommandResult(true, string.Empty, helpOutput);            
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.HelpCommand;
        }
    }
}