using System.Collections.Generic;
using System;
using System.Linq;

namespace Oppo.ObjectModel
{
    public class ObjectModel : IObjectModel
    {
        private readonly ICommandStrategyFactory _commandStrategyFactory;
        public ObjectModel(ICommandStrategyFactory commandStrategyFactory)
        {
            _commandStrategyFactory = commandStrategyFactory;
        }

        public string ExecuteCommand(IEnumerable<string> inputParams)
        {
            if (inputParams == null)
            {
                throw new ArgumentNullException(nameof(inputParams));
            }

            var inputParamsArray = inputParams.ToArray();
            var strategy = _commandStrategyFactory.GetStrategy(inputParamsArray.First());
            return strategy.Execute(inputParamsArray.Skip(1));
        }

        public void Test()
        {
            System.Console.WriteLine("coverage failure");
        }
    }
}