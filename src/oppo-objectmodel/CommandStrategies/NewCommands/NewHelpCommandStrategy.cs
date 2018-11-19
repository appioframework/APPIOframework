using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewHelpCommandStrategy : ICommand<NewStrategy>
    {
        private readonly MessageLines _helpText;

        public NewHelpCommandStrategy(string newHelpCommandName, MessageLines helpText)
        {
            Name = newHelpCommandName;
            _helpText = helpText;
        }

        public string Name {get; private set;}

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var messages = new MessageLines(_helpText);
                        
            OppoLogger.Info(LoggingText.OppoHelpForNewCommandCalled);
            return new CommandResult(true, messages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}