using System.Collections.Generic;
using System.Linq;
using Oppo.ObjectModel.CommandStrategies.NewCommands;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class NewStrategy : ICommandStrategy
    {
        private readonly INewCommandStrategyFactory _factory;

        public NewStrategy(INewCommandStrategyFactory factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.New;

        public string Execute(IEnumerable<string> inputsParams)
        {
            var inputsParamsArray = inputsParams.ToArray();
            var commandName = inputsParamsArray.First();
            var commandParams = inputsParamsArray.Skip(1).ToArray();

            var strategy = _factory.GetStrategy(commandName);
            var result = strategy.Execute(commandParams);

            return result;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.NewCommand;
        }
    }
}
