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
            var nameFlag = inputParamsList.ElementAtOrDefault(0);
            var opcuaAppName = inputParamsList.ElementAtOrDefault(1);
            var pathFlag = inputParams.ElementAtOrDefault(2);
            var modelPath = inputParamsList.ElementAtOrDefault(3);
            var outputMessages = new MessageLines();

            if (nameFlag != Constants.ImportInformationModelCommandArguments.Name && nameFlag != Constants.ImportInformationModelCommandArguments.VerboseName)
            {
                OppoLogger.Warn(LoggingText.UnknownImportInfomrationModelCommandParam);
                outputMessages.Add(OutputText.ImportInformationModelCommandUnknownParamFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // opcuaapp name validation
            if (string.IsNullOrEmpty(opcuaAppName))
            {
                OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
                outputMessages.Add(OutputText.ImportInformationModelCommandUnknownParamFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
                outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // -s flag (temporary solution for now -> needs bigger design changes)
            if (pathFlag == Constants.ImportInformationModelCommandArguments.Sample || pathFlag == Constants.ImportInformationModelCommandArguments.VerboseSample)
            {
                var content = _fileSystem.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName);
                var modelsDir = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);
                var modelFilePath = _fileSystem.CombinePaths(modelsDir, Constants.FileName.SampleInformationModelFile);
                _fileSystem.CreateFile(modelFilePath, content);
                outputMessages.Add(string.Format(OutputText.ImportSampleInformationModelSuccess, Constants.FileName.SampleInformationModelFile), string.Empty);
                OppoLogger.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, Constants.FileName.SampleInformationModelFile));
                return new CommandResult(true, outputMessages);
            }

            // path flag validation
            if (pathFlag != Constants.ImportInformationModelCommandArguments.Path && pathFlag != Constants.ImportInformationModelCommandArguments.VerbosePath)
            {
                OppoLogger.Warn(LoggingText.UnknownImportInfomrationModelCommandParam);
                outputMessages.Add(OutputText.ImportInformationModelCommandUnknownParamFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // model path validation
            if (string.IsNullOrEmpty(modelPath))
            {
                OppoLogger.Warn(LoggingText.InvalidInformationModelMissingModelFile);
                outputMessages.Add(OutputText.ImportInformationModelCommandMissingModelPath, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (_fileSystem.GetInvalidPathChars().Any(modelPath.Contains))
            {
                OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelPath, modelPath));
                outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidModelPath, modelPath), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (!_fileSystem.FileExists(modelPath))
            {
                OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelNotExistingPath, modelPath));
                outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandNotExistingModelPath, modelPath), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // model file name/extension validation
            var modelFileName = _fileSystem.GetFileName(modelPath);
            if (_fileSystem.GetExtension(modelPath) != Constants.FileExtension.InformationModel)
            {
                OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelExtension, modelFileName));
                outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidModelExtension, modelFileName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var modelsDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);            
            var targetModelFilePath = _fileSystem.CombinePaths(modelsDirectory, modelFileName);
            _fileSystem.CopyFile(modelPath, targetModelFilePath);

            OppoLogger.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelPath));
            outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandSuccess, modelPath), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}