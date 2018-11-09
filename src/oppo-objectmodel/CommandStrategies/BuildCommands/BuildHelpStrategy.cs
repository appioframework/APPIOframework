using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildHelpStrategy : ICommand<BuildStrategy>
    {
        private readonly IWriter _writer;
        private readonly IEnumerable<KeyValuePair<string, string>> _helpText;

        public BuildHelpStrategy(string buildHelpCommandName, IWriter writer, IEnumerable<KeyValuePair<string, string>> helpText)
        {
            _writer = writer;
            _helpText = helpText;
            Name = buildHelpCommandName;
        }

        public string Name { get; private set; }

        public string Execute(IEnumerable<string> inputParams)
        {
            var buildHelpOutput = new Dictionary<string, string>(_helpText);

            buildHelpOutput.Add("-n", "Project name");
            buildHelpOutput.Add("--name", "Project name");
            buildHelpOutput.Add("-h", "Build help");
            buildHelpOutput.Add("--help", "Build help");

            _writer.WriteLine("Arguments:");

            _writer.WriteLines(buildHelpOutput);
            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildHelpArgumentCommandDescription;
        }
    }
}