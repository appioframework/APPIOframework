using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel.CommandStrategies.NewCommands
{
    public class NewStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<NewStrategy> _factory;

        public NewStrategy(ICommandFactory<NewStrategy> factory)
        {
            _factory = factory;
        }
        
        public string Name => Constants.CommandName.New;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.NewCommand;
        }
    }
}
