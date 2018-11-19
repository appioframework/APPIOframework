using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishHelpStrategy : ICommand<PublishStrategy>
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _helpText;

        public PublishHelpStrategy(string publishHelpCommandName, IEnumerable<KeyValuePair<string, string>> helpText)
        {
            Name = publishHelpCommandName;
            _helpText = helpText;
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new List<KeyValuePair<string, string>>(_helpText);                       
            OppoLogger.Info(LoggingText.OpcuaappPublishHelpCalled);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}