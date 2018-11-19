using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class DeployStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<DeployStrategy> _factory;

        public DeployStrategy(ICommandFactory<DeployStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Deploy;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var commandName = inputParamsArray.ElementAt(0);
            var commandParams = inputParamsArray.Skip(1).ToArray();
            var command = _factory.GetCommand(commandName);
            var result = command.Execute(commandParams);
            return result;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.DeployStrategy;
        }
    }
}