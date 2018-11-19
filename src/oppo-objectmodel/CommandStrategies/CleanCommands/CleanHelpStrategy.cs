using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanHelpStrategy : ICommand<CleanStrategy>
    {
        private readonly MessageLines _helpText;

        public CleanHelpStrategy(string name, MessageLines helpText)
        {
            _helpText = helpText;
            Name = name;
        }

        public ICommandFactory<CleanStrategy> CommandFactory { get; set; }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            OppoLogger.Info(Resources.text.logging.LoggingText.OppoHelpForCleanCommandCalled);
            var outputMessages = new MessageLines(_helpText);
                        
            foreach (var command in CommandFactory.Commands)
            {
                outputMessages.Add(command.Name, command.GetHelpText());
            }
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanHelpArgumentCommandDescription;
        }
    }
}
