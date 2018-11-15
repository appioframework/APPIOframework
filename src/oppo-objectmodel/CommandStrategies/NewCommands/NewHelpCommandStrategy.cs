using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewHelpCommandStrategy : ICommand<NewStrategy>
    {
        private readonly IWriter _writer;

        public NewHelpCommandStrategy(string newHelpCommandName, IWriter writer)
        {
            _writer = writer;
            Name = newHelpCommandName;
        }

        public string Name {get; private set;}

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
            OppoLogger.Info(LoggingText.OppoHelpForNewCommandCalled);
            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}
