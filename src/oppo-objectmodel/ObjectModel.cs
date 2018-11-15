using System.Collections.Generic;
using System;
using System.Linq;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel
{
    public class ObjectModel : IObjectModel
    {      
        private readonly ICommandFactory<ObjectModel> _commandStrategyFactory;

        public ObjectModel(ICommandFactory<ObjectModel> commandStrategyFactory)
        {
            _commandStrategyFactory = commandStrategyFactory;
        }              

        public CommandResult ExecuteCommand(IEnumerable<string> inputParams)
        {
            if (inputParams == null)
            {
                try
                {
                    throw new ArgumentNullException(nameof(inputParams));                    
                }
                catch (Exception ex)
                {
                    OppoLogger.Error(LoggingText.NullInputParams_Msg, ex);
                    throw;
                }                
            }

            var inputParamsArray = inputParams.ToArray();
            var strategy = _commandStrategyFactory.GetCommand(inputParamsArray.FirstOrDefault());
            return strategy.Execute(inputParamsArray.Skip(1));
        }
    }
}