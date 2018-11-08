using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies.HelpCommands
{
    public class HelpStrategy : ICommand<ObjectModel>
    {
        private readonly IWriter _writer;

        public HelpStrategy(IWriter writer)
        {
            _writer = writer;
        }

        public ICommandFactory<ObjectModel> CommandFactory { get; set; }

        public virtual string Name => Constants.CommandName.Help;

        public string Execute(IEnumerable<string> inputParams)
        {
            var helpOutput = new Dictionary<string, string>();
            _writer.WriteLine(Resources.text.help.HelpTextValues.HelpStartCommand);
            
            foreach (var command in CommandFactory?.Commands ?? new ICommand<ObjectModel>[0])
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