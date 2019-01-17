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
            var modelTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName);
            
            var sourceModelRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, modelFullName);

            var pythonResult = _fileSystem.CallExecutable(Constants.ExecutableName.NodsetCompiler, srcDirectory, string.Format(Constants.ExecutableName.NodsetCompilerArguments, sourceModelRelativePath, modelTargetLocation));
            if (!pythonResult)
            {
                OppoLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailure, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            AdjustModelsTemplate(srcDirectory, modelName);

            outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, modelFullName), string.Empty);
            OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
            return new CommandResult(true, outputMessages);            
        }

        /// <summary>
        /// Models.c template should have an #include line for-each generated information-model source code.
        /// Like for myModel.c, myModel.h -> #include "information-models/mmyModel.c".
        /// </summary>
        /// <param name="srcDirectory"></param>
        /// <param name="fileNameToInclude"></param>
        private void AdjustModelsTemplate(string srcDirectory, string fileNameToInclude)
        {
            var includeSnippet = "#include \""+ Constants.DirectoryName.InformationModels + "/" + fileNameToInclude + Constants.FileExtension.CFile + "\"";

            var modelsFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_models_c));
            var currentFileContentLineByLine = ReadFileContent(modelsFileStream);

            if (!currentFileContentLineByLine.Contains(includeSnippet))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(modelsFileStream);
                foreach (var previousTextLine in currentFileContentLineByLine)
                {
                    sw.WriteLine(previousTextLine);
                }
                
                sw.WriteLine(includeSnippet);
                sw.Close();
                sw.Dispose();
            }
            modelsFileStream.Close();
            modelsFileStream.Dispose();
        }

        private IEnumerable<string> ReadFileContent(System.IO.Stream stream)
        {
            var fileContent = new List<string>();
            var sr = new System.IO.StreamReader(stream);
            string lineOfText;
            while ((lineOfText = sr.ReadLine()) != null)
            {
                fileContent.Add(lineOfText);
            }

            stream.Position = 0;
            return fileContent;
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
            return Resources.text.help.HelpTextValues.GenerateInformationModelCommandDescription;
        }
    }
}