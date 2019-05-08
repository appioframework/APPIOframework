using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<BuildStrategy> _factory;

        public BuildStrategy(ICommandFactory<BuildStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Build;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildCommand;
        }
    }
}
