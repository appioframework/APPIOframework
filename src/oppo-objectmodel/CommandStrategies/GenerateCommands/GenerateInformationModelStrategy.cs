using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System;

namespace Oppo.ObjectModel.CommandStrategies.GenerateCommands
{
    public class GenerateInformationModelStrategy : ICommand<GenerateStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public GenerateInformationModelStrategy(string commandName, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = commandName;
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsList = inputParams.ToList();
            var nameFlag = inputParamsList.ElementAtOrDefault(0);
            var opcuaAppName = inputParamsList.ElementAtOrDefault(1);
            var modelFlag = inputParamsList.ElementAtOrDefault(2);
            var modelFullName = inputParamsList.ElementAtOrDefault(3);

            var outputMessages = new MessageLines();

            if (nameFlag != Constants.GenerateInformationModeCommandArguments.Name && nameFlag != Constants.GenerateInformationModeCommandArguments.VerboseName)
            {
                OppoLogger.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, nameFlag));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, nameFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (modelFlag != Constants.GenerateInformationModeCommandArguments.Model && modelFlag != Constants.GenerateInformationModeCommandArguments.VerboseModel)
            {
                OppoLogger.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, modelFlag));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, modelFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (string.IsNullOrEmpty(opcuaAppName))
            {
                OppoLogger.Warn(LoggingText.GenerateInformationModelFailureEmptyOpcuaAppName);
                outputMessages.Add(OutputText.GenerateInformationModelFailureEmptyOpcuaAppName, string.Empty);

                return new CommandResult(false, outputMessages);
            }

            // check if model file exists
            var calculatedModelFilePath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, modelFullName);
            if (!_fileSystem.FileExists(calculatedModelFilePath))
            {
                OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, calculatedModelFilePath));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureMissingModel, modelFullName, calculatedModelFilePath), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // check if model file is an *.xml file
            var modelFileExtension = _fileSystem.GetExtension(modelFullName);
            if (modelFileExtension != Constants.FileExtension.InformationModel)
            {
                OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, modelFullName));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureInvalidModel, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var srcDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);

            // create inside src/server the information-models directory if it is not already created
            CreateNeededDirectories(srcDirectory);

            var modelName = _fileSystem.GetFileNameWithoutExtension(modelFullName);
            var args = modelFullName + " " + modelName; 
            var pythonResult = _fileSystem.CallExecutable(Constants.ExecutableName.NodsetCompiler, srcDirectory, args);
            if (!pythonResult)
            {
                OppoLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailure, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            AdjustServerTemplate(srcDirectory, modelName);

            outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, modelFullName), string.Empty);
            OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
            return new CommandResult(true, outputMessages);            
        }

        /// <summary>
        /// Server template should call the (from information-model) generated code. 
        /// </summary>
        /// <param name="srcDirectory"></param>
        private void AdjustServerTemplate(string srcDirectory, string modelName)
        {
            
        }

        private void CreateNeededDirectories(string srcDirectory)
        {
            var pathToCreate = System.IO.Path.Combine(srcDirectory, Constants.DirectoryName.InformationModels);
            if (!_fileSystem.DirectoryExists(pathToCreate))
            {
                _fileSystem.CreateDirectory(pathToCreate);
            }            
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.GenerateInformationModelArgumentCommandDescription;
        }
    }
}