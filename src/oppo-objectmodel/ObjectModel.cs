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
                try
                {
                    throw new ArgumentNullException(nameof(inputParams));                    
                }
                catch (Exception ex)
                {
                    OppoLogger.Error(Resources.text.logging.LoggingText.NullInputParams_Msg, ex);
                    throw;
                }                
            }

            var inputParamsArray = inputParams.ToArray();
            var strategy = _commandStrategyFactory.GetCommand(inputParamsArray.FirstOrDefault());
            return strategy.Execute(inputParamsArray.Skip(1));
        }
    }
}