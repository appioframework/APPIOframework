using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportInformationModelCommandStrategy : ICommand<ImportStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public ImportInformationModelCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.ImportInformationModelCommandName.IModel;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsList = inputParams.ToList();
            var projectName = inputParamsList.ElementAtOrDefault(0);
            
            var modelPath = inputParamsList.ElementAtOrDefault(2);
            var outputMessages = new MessageLines();

            var modelsDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Models);
            _fileSystem.CopyFile(modelPath, modelsDirectory);

            OppoLogger.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelPath));
            outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandSuccess, modelPath), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}