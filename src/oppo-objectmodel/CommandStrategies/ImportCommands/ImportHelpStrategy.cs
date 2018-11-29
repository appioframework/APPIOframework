using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportHelpStrategy : ICommand<ImportStrategy>
    {

        private MessageLines _helpText;

        public ImportHelpStrategy(string commandName, MessageLines helpText)
        {

            _helpText = helpText;
            Name = commandName;
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var outputMessages = new MessageLines(_helpText);
            return new CommandResult(true,outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.ImportHelpArgumentCommandDescription;
        }
    }
}
