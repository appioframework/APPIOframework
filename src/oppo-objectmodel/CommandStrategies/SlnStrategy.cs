using System.Collections.Generic;
using System.Linq;

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

        public string Execute(IEnumerable<string> inputsArgs)
        {
            var strategy = _factory.GetStrategy(inputsArgs.FirstOrDefault());
            return strategy.Execute(inputsArgs.Skip(1));
        }
    }
}