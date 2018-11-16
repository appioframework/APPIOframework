using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishHelpStrategy : ICommand<PublishStrategy>
    {
        public PublishHelpStrategy(string publishHelpCommandName)
        {
            Name = publishHelpCommandName;
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var buildHelpOutput = new Dictionary<string, string>();

            buildHelpOutput.Add("Arguments:", string.Empty);
            buildHelpOutput.Add("<Project>", "The project directory to use");
            buildHelpOutput.Add(" ", "");
            buildHelpOutput.Add("Options:", "");
            buildHelpOutput.Add("-n", "Project name");
            buildHelpOutput.Add("--name", "Project name");
            buildHelpOutput.Add("-h", "Publish help");
            buildHelpOutput.Add("--help", "Publish help");
            
            OppoLogger.Info(LoggingText.OpcuaappPublishHelpCalled);
            return new CommandResult(true, string.Empty, buildHelpOutput);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}