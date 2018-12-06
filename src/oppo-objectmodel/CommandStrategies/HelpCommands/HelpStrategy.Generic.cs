using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.HelpCommands
{
    public class HelpStrategy<TDependance> : ICommand<TDependance>
    {
        private readonly HelpData _helpData;

        public HelpStrategy(HelpData helpData)
        {
            _helpData = helpData.Clone();
        }

        public ICommandFactory<TDependance> CommandFactory { get; set; }

        public string Name => _helpData.CommandName;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var commandLines = new MessageLines();
            foreach (var command in CommandFactory?.Commands ?? new ICommand<TDependance>[0])
            {
                commandLines.Add(command.Name, command.GetHelpText());
            }
            commandLines.Sort();

            var outputMessages = new MessageLines
            {
                _helpData.HelpTextFirstLine,
                commandLines,
                _helpData.HelpTextLastLine
            };

            OppoLogger.Info(_helpData.LogMessage);
            return new CommandResult(true, outputMessages);            
        }

        public string GetHelpText()
        {
            return _helpData.HelpText;
        }
    }
}