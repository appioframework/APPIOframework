using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanHelpStrategy : ICommand<CleanStrategy>
    {
        private readonly KeyValuePair<string, string>[] _helpLines;

        public CleanHelpStrategy(string name, IEnumerable<KeyValuePair<string, string>> helpText)
        {
            _helpLines = helpText.ToArray();
            Name = name;
        }

        public ICommandFactory<CleanStrategy> CommandFactory { get; set; }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            OppoLogger.Info(Resources.text.logging.LoggingText.OppoHelpForCleanCommandCalled);
            var outputText = new Dictionary<string, string>(_helpLines);
            foreach (var command in CommandFactory.Commands)
            {
                outputText.Add(command.Name, command.GetHelpText());
            }
            return new CommandResult(true, string.Empty, outputText);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanHelpArgumentCommandDescription;
        }
    }
}
