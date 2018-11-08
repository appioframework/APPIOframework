using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<NewStrategy> _factory;

        public NewStrategy(ICommandFactory<NewStrategy> factory)
        {
            _factory = factory;
        }

        [ExcludeFromCodeCoverage]
        public string Name => Constants.CommandName.New;

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputsParamsArray = inputParams.ToArray();
            var commandName = inputsParamsArray.First();
            var commandParams = inputsParamsArray.Skip(1).ToArray();
            var strategy = _factory.GetCommand(commandName);
            var result = strategy.Execute(commandParams);
            return result;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.NewCommand;
        }
    }
}
