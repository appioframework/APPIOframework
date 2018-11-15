using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishHelpStrategy : ICommand<PublishStrategy>
    {
        private readonly IWriter _writer;

        public PublishHelpStrategy(string publishHelpCommandName, IWriter writer)
        {
            _writer = writer;
            Name = publishHelpCommandName;
        }

        public string Name { get; private set; }

        public string Execute(IEnumerable<string> inputParams)
        {
            var buildHelpOutput = new Dictionary<string, string>();

            buildHelpOutput.Add("<Project>", "The project directory to use");
            buildHelpOutput.Add(" ", "");
            buildHelpOutput.Add("Options:", "");
            buildHelpOutput.Add("-n", "Project name");
            buildHelpOutput.Add("--name", "Project name");
            buildHelpOutput.Add("-h", "Publish help");
            buildHelpOutput.Add("--help", "Publish help");

            _writer.WriteLine("Arguments:");

            _writer.WriteLines(buildHelpOutput);
            OppoLogger.Info(LoggingText.OpcuaappPublishHelpCalled);
            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}