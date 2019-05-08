using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.DeployCommands
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
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.DeployStrategy;
        }
    }
}