using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishHelpStrategy : ICommand<PublishStrategy>
    {
        private readonly IWriter _writer;

        public PublishHelpStrategy(IWriter writer)
        {
            _writer = writer;
        }

        public virtual string Name => Constants.PublishCommandArguments.Help;

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
            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}
