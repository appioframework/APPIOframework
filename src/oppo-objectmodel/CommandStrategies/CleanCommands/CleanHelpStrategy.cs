using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanHelpStrategy : ICommand<CleanStrategy>
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _helpText;

        public CleanHelpStrategy(string name, IEnumerable<KeyValuePair<string, string>> helpText)
        {
            _helpText = helpText;
            Name = name;
        }

        public ICommandFactory<CleanStrategy> CommandFactory { get; set; }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            OppoLogger.Info(Resources.text.logging.LoggingText.OppoHelpForCleanCommandCalled);
            var outputMessages = new List<KeyValuePair<string, string>>(_helpText);
            foreach (var command in CommandFactory.Commands)
            {
                outputMessages.Add(new KeyValuePair<string, string>(command.Name, command.GetHelpText()));
            }
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanHelpArgumentCommandDescription;
        }
    }
}
