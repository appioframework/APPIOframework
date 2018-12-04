using System;
using System.Collections.Generic;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportInformationModelSamplesStrategy : ICommand<ImportStrategy>
    {
        private readonly string _name;
        public ImportInformationModelSamplesStrategy(string name)
        {
            _name = name;
        }
        public string Name =>_name;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            throw new NotImplementedException();
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.ImportSamplesArgumentCommandDescription;
        }
    }
}
