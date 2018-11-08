using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewHelpCommandStrategy : ICommand<NewStrategy>
    {
        private readonly IWriter _writer;

        public NewHelpCommandStrategy(IWriter writer)
        {
            _writer = writer;
        }

        public virtual string Name => Constants.NewCommandName.Help;

        public string Execute(IEnumerable<string> inputParams)
        {
            var buildHelpOutput = new Dictionary<string, string>();

            buildHelpOutput.Add("<Object>", "The object to create, can either be:");
            buildHelpOutput.Add("   ", "  sln");
            buildHelpOutput.Add("  ", "  opcuaapp");
            buildHelpOutput.Add(" ", "");
            buildHelpOutput.Add("Options:", "");
            buildHelpOutput.Add("-n", "Name of the object to create");
            buildHelpOutput.Add("--name", "Name of the object to create");
            buildHelpOutput.Add("-h", "New help");
            buildHelpOutput.Add("--help", "New help");

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
