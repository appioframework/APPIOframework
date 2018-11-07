using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.HelpCommands
{
    public class HelpStrategy : ICommandStrategy
    {
        private IWriter _writer;
        private readonly List<ICommandStrategy> _availableCommands;

        public HelpStrategy(IWriter writer, List<ICommandStrategy> availableCommands)
        {
            _writer = writer;
            _availableCommands = availableCommands;
        }

        public string Execute(IEnumerable<string> inputsParams)
        {
            var helpOutput = new List<string>();
            helpOutput.Add(Resources.text.help.HelpText.HelpStartCommand);
            foreach (var command in _availableCommands)
            {                
                helpOutput.Add(command.GetHelpText());
            }

            helpOutput.Add(Resources.text.help.HelpText.HelpEndCommand); 
            _writer.WriteLines(helpOutput);
            return Constants.CommandResults.Success;            
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}