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

        public string Name => Constants.ImportInformationModelCommandName.InformationModel;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsList = inputParams.ToList();
            var opcuaAppName = inputParamsList.ElementAtOrDefault(0);
            var pathFlag = inputParams.ElementAtOrDefault(1);
            var modelPath = inputParamsList.ElementAtOrDefault(2);
            var outputMessages = new MessageLines();

            // opcuaapp name validation
            if (string.IsNullOrEmpty(opcuaAppName))
            {
                OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
                outputMessages.Add(OutputText.ImportInforamtionModelCommandUnknownParamFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
                outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandInvalidOpcuaappName, opcuaAppName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // path flag validation
            if (pathFlag != Constants.ImportInformationModelCommandArguments.Path && pathFlag != Constants.ImportInformationModelCommandArguments.VerbosePath)
            {
                OppoLogger.Warn(LoggingText.UnknownImportInfomrationModelCommandParam);
                outputMessages.Add(OutputText.ImportInforamtionModelCommandUnknownParamFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // model path validation
            if (_fileSystem.GetInvalidPathChars().Any(modelPath.Contains))
            {
                OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelPath, modelPath));
                outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandInvalidModelPath, modelPath), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (!_fileSystem.FileExists(modelPath))
            {
                OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelNotExistingPath, modelPath));
                outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandNotExistingModelPath, modelPath), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // model file name/extension validation
            var modelFileName = _fileSystem.GetFileName(modelPath);
            if (_fileSystem.GetExtension(modelPath) != Constants.FileExtension.InformationModel)
            {
                OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelExtension, modelFileName));
                outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandInvalidModelExtension, modelFileName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var modelsDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);            
            var targetModelFilePath = _fileSystem.CombinePaths(modelsDirectory, modelFileName);
            _fileSystem.CopyFile(modelPath, targetModelFilePath);

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