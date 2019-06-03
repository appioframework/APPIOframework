using System.Collections.Generic;
using System.Linq;

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
			if (_helpData.Arguments.Count() != 0)
			{
				_helpData.Arguments.Sort();
				_helpData.Arguments = new MessageLines()
				{
					{string.Empty, string.Empty },
					{ Resources.text.help.HelpTextValues.GeneralArguments, string.Empty },
					_helpData.Arguments
				};
			}

			if (_helpData.Options.Count() != 0)
			{
				_helpData.Options.Sort();
				_helpData.Options = new MessageLines()
				{
					{string.Empty, string.Empty },
					{ Resources.text.help.HelpTextValues.GeneralOptions, string.Empty },
					_helpData.Options
				};
			}

            var outputMessages = new MessageLines
            {
                _helpData.HelpTextFirstLine,
				_helpData.Arguments,
				_helpData.Options,
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