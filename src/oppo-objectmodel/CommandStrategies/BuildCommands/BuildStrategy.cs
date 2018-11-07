using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildStrategy : ICommandStrategy
    {
        private readonly IBuildCommandStrategyFactory _factory;

        public BuildStrategy(IBuildCommandStrategyFactory factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Build;

        public string Execute(IEnumerable<string> inputsParams)
        {
            var inputParamsArray = inputsParams.ToArray();

            var buildStrategy = _factory.GetStrategy(inputParamsArray.ElementAtOrDefault(0));
            return buildStrategy.Execute(inputParamsArray.Skip(1));          
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildCommand;
        }
    }
}
