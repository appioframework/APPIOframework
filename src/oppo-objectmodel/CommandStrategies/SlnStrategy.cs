using System.Collections.Generic;
using System.Linq;
using Oppo.ObjectModel.CommandStrategies.SlnCommands;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class SlnStrategy
        : ICommandStrategy
    {
        private readonly ISlnCommandStrategyFactory _factory;

        public SlnStrategy(ISlnCommandStrategyFactory factory)
        {
            _factory = factory;
        }

        public string Execute(IEnumerable<string> inputsParams)
        {
            var inputsArgsArray = inputsParams.ToArray();
            var strategy = _factory.GetStrategy(inputsArgsArray.FirstOrDefault());
            return strategy.Execute(inputsArgsArray.Skip(1));
        }
    }
}