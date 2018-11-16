using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewHelpCommandStrategy : ICommand<NewStrategy>
    {
        public NewHelpCommandStrategy(string newHelpCommandName)
        {
            Name = newHelpCommandName;
        }

        public string Name {get; private set;}

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var buildHelpOutput = new Dictionary<string, string>();

            buildHelpOutput.Add("Arguments:", string.Empty);
            buildHelpOutput.Add("<Object>", "The object to create, can either be:");
            buildHelpOutput.Add("   ", "sln");
            buildHelpOutput.Add("  ", "opcuaapp");
            buildHelpOutput.Add(" ", "");
            buildHelpOutput.Add("Options:", "");
            buildHelpOutput.Add("-n", "Name of the object to create");
            buildHelpOutput.Add("--name", "Name of the object to create");
            buildHelpOutput.Add("-h", "New help");
            buildHelpOutput.Add("--help", "New help");

            var messages = new List<KeyValuePair<string, string>>();
            messages.Add(new KeyValuePair<string, string>("Arguments:", string.Empty));
            messages.Add(new KeyValuePair<string, string>("<Object>", "The object to create, can either be:"));
            messages.Add(new KeyValuePair<string, string>(string.Empty, "sln"));
            messages.Add(new KeyValuePair<string, string>(string.Empty, "opcuaapp"));
            messages.Add(new KeyValuePair<string, string>(string.Empty, ""));
            messages.Add(new KeyValuePair<string, string>("Options:", ""));
            messages.Add(new KeyValuePair<string, string>("-n", "Name of the object to create"));
            messages.Add(new KeyValuePair<string, string>("--name", "Name of the object to create"));
            messages.Add(new KeyValuePair<string, string>("-h", "New help"));
            messages.Add(new KeyValuePair<string, string>("--help", "New help"));

            OppoLogger.Info(LoggingText.OppoHelpForNewCommandCalled);
            return new CommandResult(true, string.Empty, buildHelpOutput, messages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}