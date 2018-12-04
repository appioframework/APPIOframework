using Oppo.Resources.text.help;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportStrategy : ICommand<ObjectModel>
    {
        private readonly ICommandFactory<ImportStrategy> _factory;

        public ImportStrategy(ICommandFactory<ImportStrategy> factory)
        {
            _factory = factory;
        }

      
        public string Name => Constants.CommandName.Import;
        
        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsList = inputParams.ToList();
            var commandName = inputParamsList.FirstOrDefault();
            var command = _factory.GetCommand(commandName);
            var commandParamters = inputParamsList.Skip(1);
            return command.Execute(commandParamters);            
        }

        public string GetHelpText()
        {
            return HelpTextValues.ImportStrategy;
        }
    }
}