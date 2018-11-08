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

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();

            var buildStrategy = _factory.GetCommand(inputParamsArray.ElementAtOrDefault(0));
            return buildStrategy.Execute(inputParamsArray.Skip(1));          
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildCommand;
        }
    }
}
