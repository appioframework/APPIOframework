using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishHelpStrategy : ICommand<PublishStrategy>
    {
        private readonly MessageLines _helpText;

        public PublishHelpStrategy(string publishHelpCommandName, MessageLines helpText)
        {
            Name = publishHelpCommandName;
            _helpText = helpText;
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines(_helpText);
                        
            OppoLogger.Info(LoggingText.OpcuaappPublishHelpCalled);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}