using Oppo.Resources.text.logging;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildHelpStrategy : ICommand<BuildStrategy>
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _helpText;

        public BuildHelpStrategy(string buildHelpCommandName, IEnumerable<KeyValuePair<string, string>> helpText)
        {
            _helpText = helpText;
            Name = buildHelpCommandName;
        }

        public string Name { get; private set; }

        public ICommandFactory<BuildStrategy> CommandFactory { get; set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new List<KeyValuePair<string, string>>(_helpText);            
            foreach (var command in CommandFactory.Commands)
            {            
                outputMessages.Add(new KeyValuePair<string, string>(command.Name, command.GetHelpText()));
            }

            OppoLogger.Info(LoggingText.OppoHelpForBuildCommandCalled);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildHelpArgumentCommandDescription;
        }
    }
}