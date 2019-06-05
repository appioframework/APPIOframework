using Appio.Resources.text.help;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Appio.ObjectModel.CommandStrategies.ImportCommands
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
            return ParameterResolver.DelegateToSubcommands(_factory, inputParams);         
        }

        public string GetHelpText()
        {
            return HelpTextValues.ImportCommand;
        }
    }
}