using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<ObjectModel> _factory;

        public CleanStrategy(ICommandFactory<ObjectModel> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Clean;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var commandName = inputParamsArray.ElementAt(0);
            var commandParams = inputParamsArray.Skip(1).ToArray();
            var command = _factory.GetCommand(commandName);
            return command.Execute(commandParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanCommand;
        }
    }
}
