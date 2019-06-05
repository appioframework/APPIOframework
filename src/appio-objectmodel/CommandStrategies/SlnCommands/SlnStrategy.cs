using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel.CommandStrategies.SlnCommands
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
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.SlnCommand;
        }
    }
}
