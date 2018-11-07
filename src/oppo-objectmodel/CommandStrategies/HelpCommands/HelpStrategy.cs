using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.ObjectModel.CommandStrategies.HelpCommands
{
    public class HelpStrategy : ICommandStrategy
    {
        private readonly IWriter _writer;
        private readonly List<ICommandStrategy> _availableCommands;

        public HelpStrategy(IWriter writer, List<ICommandStrategy> availableCommands)
        {
            _writer = writer;
            _availableCommands = availableCommands;
        }

        [ExcludeFromCodeCoverage]
        public string Name => Constants.CommandName.Help;

        public string Execute(IEnumerable<string> inputsParams)
        {
            var helpOutput = new Dictionary<string, string>();
            _writer.WriteLine(Resources.text.help.HelpTextValues.HelpStartCommand);
            
            helpOutput.Add(Name, GetHelpText());
            foreach (var command in _availableCommands)
            {                
                helpOutput.Add(command.Name, command.GetHelpText());
            }
            
            _writer.WriteLines(helpOutput);
            _writer.WriteLine(Resources.text.help.HelpTextValues.HelpEndCommand);
            return Constants.CommandResults.Success;            
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.HelpCommand;
        }
    }
}