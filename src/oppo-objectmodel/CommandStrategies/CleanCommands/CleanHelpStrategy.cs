using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanHelpStrategy : ICommand<CleanStrategy>
    {
        private readonly ICommandFactory<CleanStrategy> _factory;
        private readonly KeyValuePair<string, string>[] _helpLines;

        public CleanHelpStrategy(string name, ICommandFactory<CleanStrategy> factory, IEnumerable<KeyValuePair<string, string>> helpText)
        {
            _factory = factory;
            _helpLines = helpText.ToArray();
            Name = name;
        }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputText = new Dictionary<string, string>(_helpLines);
            foreach (var command in _factory.Commands)
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
