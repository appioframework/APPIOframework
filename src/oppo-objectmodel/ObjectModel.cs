using System.Collections.Generic;
using System;
using System.Linq;

namespace Oppo.ObjectModel
{
    public class ObjectModel : IObjectModel
    {
        private readonly ICommandFactory<ObjectModel> _commandStrategyFactory;
        public ObjectModel(ICommandFactory<ObjectModel> commandStrategyFactory)
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
            var strategy = _commandStrategyFactory.GetCommand(inputParamsArray.FirstOrDefault());
            return strategy.Execute(inputParamsArray.Skip(1));
        }
    }
}