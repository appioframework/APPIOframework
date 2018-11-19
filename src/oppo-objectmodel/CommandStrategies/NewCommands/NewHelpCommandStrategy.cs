using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewHelpCommandStrategy : ICommand<NewStrategy>
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _helpText;

        public NewHelpCommandStrategy(string newHelpCommandName, IEnumerable<KeyValuePair<string, string>> helpText)
        {
            Name = newHelpCommandName;
            _helpText = helpText;
        }

        public string Name {get; private set;}

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var messages = new List<KeyValuePair<string, string>>(_helpText);

            OppoLogger.Info(LoggingText.OppoHelpForNewCommandCalled);
            return new CommandResult(true, messages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}