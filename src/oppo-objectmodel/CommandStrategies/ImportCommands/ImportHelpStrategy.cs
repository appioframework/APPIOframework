using Oppo.Resources.text.logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportHelpStrategy : ICommand<ImportStrategy>
    {

        private readonly MessageLines _helpText;

        public ImportHelpStrategy(string commandName, MessageLines helpText)
        {

            _helpText = helpText;
            Name = commandName;
        }
        public ICommandFactory<ImportStrategy> CommandFactory { get; set; }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines(_helpText);

            foreach (var command in CommandFactory.Commands)
            {
                outputMessages.Add(command.Name, command.GetHelpText());
            }

            OppoLogger.Info(LoggingText.OppoHelpForImportInformationModel);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.ImportHelpArgumentCommandDescription;
        }
    }
}
