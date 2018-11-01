using System.Collections.Generic;
using System;
using System.Linq;

namespace Oppo.ObjectModel
{
    public class ObjectModel
    {
        private readonly ICommandStrategyFactory _commandStrategyFactory;
        public ObjectModel( ICommandStrategyFactory commandStrategyFactory)
        {
            _commandStrategyFactory = commandStrategyFactory;
        }

        public string ExecuteCommand(IEnumerable<string> inputParams)
        {
            if (inputParams == null)
            {
                throw new ArgumentNullException(nameof(inputParams));
            }
           
            var strategy = _commandStrategyFactory.GetStrategy(inputParams.First());
            return strategy.Execute(inputParams.Skip(1));
        }
    }
}