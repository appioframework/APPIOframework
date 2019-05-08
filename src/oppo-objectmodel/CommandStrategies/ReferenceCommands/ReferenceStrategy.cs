using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.ReferenceCommands
{
        public class ReferenceStrategy : ICommand<ObjectModel>
        {
            private readonly ICommandFactory<ReferenceStrategy> _factory;

            public ReferenceStrategy(ICommandFactory<ReferenceStrategy> factory)
            {
                _factory = factory;
            }

            public string Name => Constants.CommandName.Reference;

            public CommandResult Execute(IEnumerable<string> inputParams)
            {
                return ParameterResolver.DelegateToSubcommands(_factory, inputParams);
            }

            public string GetHelpText()
            {
                return Resources.text.help.HelpTextValues.ReferenceCommand;
            }
        }
    
}
