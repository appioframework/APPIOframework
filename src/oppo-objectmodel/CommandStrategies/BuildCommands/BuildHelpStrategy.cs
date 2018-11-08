using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildHelpStrategy : ICommand<BuildStrategy>
    {
        private readonly IWriter _writer;

        public BuildHelpStrategy(IWriter writer)
        {
            _writer = writer;
        }

        public virtual string Name => Constants.BuildCommandArguments.Help;

        public string Execute(IEnumerable<string> inputParams)
        {
            var buildHelpOutput = new Dictionary<string, string>();
            
            buildHelpOutput.Add("<Project>", "The project directory to use");
            buildHelpOutput.Add(" ", "");
            buildHelpOutput.Add("Options:", "");
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
            return string.Empty;
        }
    }
}