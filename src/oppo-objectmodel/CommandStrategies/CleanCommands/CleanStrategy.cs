using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<CleanStrategy> _factory;

        public CleanStrategy(ICommandFactory<CleanStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Clean;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanCommand;
        }
    }
}
