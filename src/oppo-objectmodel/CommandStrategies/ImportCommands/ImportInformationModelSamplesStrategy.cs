using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportInformationModelSamplesStrategy : ICommand<ImportStrategy>
    {
        private readonly string _name;
        private readonly IFileSystem _fileSystem;
        public ImportInformationModelSamplesStrategy(IFileSystem fileSystem, string name)
        {
            _name = name;
            _fileSystem = fileSystem;
        }
        public string Name =>_name;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
          
            var content = _fileSystem.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName);
            var projectName = inputParams.ElementAt(0);
            var modelsDir = _fileSystem.CombinePaths(projectName,Constants.DirectoryName.Models);
            var modelFilePath = _fileSystem.CombinePaths(modelsDir,Constants.FileName.SampleInformationModelFile);
            _fileSystem.CreateFile(modelFilePath,content);
            return new CommandResult(true, new MessageLines());
         }

 

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.ImportSamplesArgumentCommandDescription;
        }
    }
}
