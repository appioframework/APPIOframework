using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.GenerateCommands
{
    public class GenerateStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<GenerateStrategy> _factory;

        public GenerateStrategy(ICommandFactory<GenerateStrategy> factory)
        {
            _factory = factory;
        }

        public string Name => Constants.CommandName.Generate;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();

            var generateCommand = _factory.GetCommand(inputParamsArray.ElementAtOrDefault(0));
            return generateCommand.Execute(inputParamsArray.Skip(1).ToArray());
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.GenerateCommand;
        }

    }
}
