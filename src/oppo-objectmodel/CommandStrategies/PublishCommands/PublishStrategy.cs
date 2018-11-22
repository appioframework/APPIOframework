using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<PublishStrategy> _factory;

        public PublishStrategy(ICommandFactory<PublishStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Publish;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var commandName = inputParamsArray.ElementAtOrDefault(0);
            var commandParams = inputParamsArray.Skip(1).ToArray();
            var command = _factory.GetCommand(commandName);
            var result = command.Execute(commandParams);
            return result;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.PublishCommand;
        }
    }
}