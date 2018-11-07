using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildHelpStrategy : IBuildStrategy
    {
        private readonly IWriter _writer;

        public BuildHelpStrategy(IWriter writer)
        {
            _writer = writer;
        }        

        public string Execute(IEnumerable<string> inputsParams)
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
    }
}