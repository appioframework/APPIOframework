using System.Collections.Generic;
using System;
using System.Linq;

namespace Oppo.ObjectModel
{
    public class ObjectModel : IObjectModel
    {      
        private readonly ICommandFactory<ObjectModel> _commandStrategyFactory;
        private readonly ILogger _logger;

        public ObjectModel(ICommandFactory<ObjectModel> commandStrategyFactory, ILogger logger)
        {
            _commandStrategyFactory = commandStrategyFactory;
            _logger = logger;
        }

        public string ExecuteCommand(IEnumerable<string> inputParams)
        {
            if (inputParams == null)
            {
                try
                {
                    throw new ArgumentNullException(nameof(inputParams));                    
                }
                catch (Exception ex)
                {
                    _logger.Error(Resources.text.logging.LoggingText.NullInputParams_Msg, ex);
                    throw;
                }                
            }

            var inputParamsArray = inputParams.ToArray();
            var strategy = _commandStrategyFactory.GetCommand(inputParamsArray.FirstOrDefault());
            return strategy.Execute(inputParamsArray.Skip(1));
        }
    }
}