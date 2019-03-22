using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<SlnStrategy> _factory;

        public SlnStrategy(ICommandFactory<SlnStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Sln;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var commandName = inputParamsArray.ElementAtOrDefault(0);
            var commandParams = inputParamsArray.Skip(1).ToArray();
            var command = _factory.GetCommand(commandName);
            return command.Execute(commandParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnCommand;
        }
    }
}
