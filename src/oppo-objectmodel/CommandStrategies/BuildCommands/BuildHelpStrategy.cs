using Oppo.Resources.text.logging;
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

        public ICommandFactory<BuildStrategy> CommandFactory { get; set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var buildHelpOutput = new Dictionary<string, string>(_helpText);
            foreach (var command in CommandFactory.Commands)
            {
                buildHelpOutput.Add(command.Name, command.GetHelpText());
            }

            _writer.WriteLines(buildHelpOutput);
            OppoLogger.Info(LoggingText.OppoHelpForBuildCommandCalled);
            return new CommandResult(true, string.Empty);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildHelpArgumentCommandDescription;
        }
    }
}