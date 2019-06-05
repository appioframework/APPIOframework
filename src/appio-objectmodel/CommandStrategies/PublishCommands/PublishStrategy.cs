using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel.CommandStrategies.PublishCommands
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
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.PublishCommand;
        }
    }
}