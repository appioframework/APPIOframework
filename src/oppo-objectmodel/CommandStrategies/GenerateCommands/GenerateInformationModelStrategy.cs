using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System;
using System.IO;

namespace Oppo.ObjectModel.CommandStrategies.GenerateCommands
{
    public class GenerateInformationModelStrategy : ICommand<GenerateStrategy>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IModelValidator _modelValidator;

        public GenerateInformationModelStrategy(string commandName, IFileSystem fileSystem, IModelValidator modelValidator)
        {
            _fileSystem = fileSystem;
            _modelValidator = modelValidator;
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
            var requiredFile1Flag = inputParamsList.ElementAtOrDefault(4);
            var requiredFile1FullName = inputParamsList.ElementAtOrDefault(5);
            var requiredFile2Flag = inputParamsList.ElementAtOrDefault(6);
            var requiredFile2FullName = inputParamsList.ElementAtOrDefault(7);

            var outputMessages = new MessageLines();

            if (nameFlag != Constants.GenerateInformationModeCommandArguments.Name && nameFlag != Constants.GenerateInformationModeCommandArguments.VerboseName)
            {
                OppoLogger.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, nameFlag));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, nameFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (modelFlag != Constants.GenerateInformationModeCommandArguments.Model && modelFlag != Constants.GenerateInformationModeCommandArguments.VerboseModel)
            {
                OppoLogger.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, modelFlag));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, modelFlag), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var requiredTypes = false;
            var requiredTypesFullName = string.Empty;
            var requiredModel = false;
            var requiredModelFullName = string.Empty;
            if (requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.Types || requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.VerboseTypes)
            {
                requiredTypes = true;
                requiredTypesFullName = requiredFile1FullName;
            }
            else if (requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.RequiredModel || requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.VerboseRequiredModel)
            {
                requiredModel = true;
                requiredModelFullName = requiredFile1FullName;
            }
            else if (!string.IsNullOrEmpty(requiredFile1Flag))
            {
                OppoLogger.Warn(string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, requiredFile1Flag));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, requiredFile1Flag), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (string.IsNullOrEmpty(opcuaAppName))
            {
                OppoLogger.Warn(LoggingText.GenerateInformationModelFailureEmptyOpcuaAppName);
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureEmptyOpcuaAppName, opcuaAppName, modelFullName), string.Empty);

                return new CommandResult(false, outputMessages);
            }


            // check if model file exists
            var calculatedModelFilePath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, modelFullName);
            if (!_fileSystem.FileExists(calculatedModelFilePath))
            {
                OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, calculatedModelFilePath));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureMissingModel, opcuaAppName, modelFullName, calculatedModelFilePath), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // check if required types file exists
            if (requiredTypes)
            {
                var calculatedRequiredTypesPath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredFile1FullName);
                if (!_fileSystem.FileExists(calculatedRequiredTypesPath))
                {
                    OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, calculatedRequiredTypesPath));
                    outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureMissingFile, opcuaAppName, modelFullName, calculatedRequiredTypesPath), string.Empty);
                    return new CommandResult(false, outputMessages);
                }
            }

            // check if model file is an *.xml file
            var modelFileExtension = _fileSystem.GetExtension(modelFullName);
            if (modelFileExtension != Constants.FileExtension.InformationModel)
            {
                OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, modelFullName));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureInvalidModel, opcuaAppName, modelFullName, modelFileExtension), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // check if required types file is a *.bsd
            if (requiredTypes)
            {
                var requiredTypesFileExtension = _fileSystem.GetExtension(requiredFile1FullName);
                if (requiredTypesFileExtension != Constants.FileExtension.ModelTypes)
                {
                    OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, requiredFile1FullName));
                    outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureInvalidFile, opcuaAppName, requiredFile1FullName, requiredTypesFileExtension), string.Empty);
                    return new CommandResult(false, outputMessages);
                }
            }

            // validate model
            if (!_modelValidator.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName))
            {
                OppoLogger.Warn(string.Format(LoggingText.GenerateInformationModelFailureValidatingModel, modelFullName));
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailureValidatingModel, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var srcDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);

            // create inside src/server the information-models directory if it is not already created
            CreateNeededDirectories(srcDirectory);
            
            var modelName = _fileSystem.GetFileNameWithoutExtension(modelFullName);
            var modelTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName);
            
            var sourceModelRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, modelFullName);

            var requiredTypesName = string.Empty;
            if (requiredTypes)
            {
                var requiredTypesSourceLocation = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, requiredTypesFullName);
                requiredTypesName = _fileSystem.GetFileNameWithoutExtension(requiredTypesFullName);
                var requiredTypesTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, requiredTypesName.ToLower());

                var generatedTypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath + string.Format(Constants.ExecutableName.GenerateDatatypesTypeBsd, requiredTypesSourceLocation) + " " + requiredTypesTargetLocation;
                var generatedTypesResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, generatedTypesArgs);
                if (!generatedTypesResult)
                {
                    OppoLogger.Warn(LoggingText.GeneratedTypesExecutableFails);
                    outputMessages.Add(string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, opcuaAppName, modelFullName, requiredTypesFullName), string.Empty);
                    return new CommandResult(false, outputMessages);
                }
            }
            
            var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);
            if (requiredTypes)
            {
                nodesetCompilerArgs += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, requiredTypesName.ToUpper());
            }
            else
            {
                nodesetCompilerArgs += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);
            }
            nodesetCompilerArgs += string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) + string.Format(Constants.ExecutableName.NodesetCompilerXml, sourceModelRelativePath, modelTargetLocation);
            var nodesetResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs);
            if (!nodesetResult)
            {
                OppoLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailure, opcuaAppName, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            AdjustModelsTemplate(srcDirectory, modelName);
            if (requiredTypes)
            {
                AdjustModelsTemplate(srcDirectory, requiredTypesName.ToLower() + "_generated");
            }
            AdjustNodeSetFunctionsTemplate(srcDirectory, modelName);

            outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, opcuaAppName, modelFullName), string.Empty);
            OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
            return new CommandResult(true, outputMessages);           
        }
        
        /// <summary>
        /// Models.c template should have an #include line for-each generated information-model source code.
        /// Like for myModel.c, myModel.h -> #include "information-models/mmyModel.c".
        /// </summary>
        /// <param name="srcDirectory">Server source directory for current opcuaapp project.</param>
        /// <param name="fileNameToInclude">Generated file name without extension to be included.</param>
        private void AdjustModelsTemplate(string srcDirectory, string fileNameToInclude)
        {
            var includeSnippet = Constants.IncludeSnippet + " \"" + Constants.DirectoryName.InformationModels + "/" + fileNameToInclude + Constants.FileExtension.CFile + "\"";

            var modelsFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_models_c));
            var currentFileContentLineByLine = ReadFileContent(modelsFileStream);

            if (!currentFileContentLineByLine.Contains(includeSnippet))
            {
                var sw = new StreamWriter(modelsFileStream);
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

        private void AdjustNodeSetFunctionsTemplate(string srcDirectory, string functionName)
        {
            var functionSnippet = string.Format(Constants.NodeSetFunctioncContent.FunctionSnippetPart1,functionName);
            
            var nodeSetFunctioncsFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_nodeSetFunctions_c));
            var currentFileContentLineByLine = ReadFileContent(nodeSetFunctioncsFileStream).ToList<string>();
            
            nodeSetFunctioncsFileStream.Close();
            nodeSetFunctioncsFileStream.Dispose();

            if (!currentFileContentLineByLine.Contains(functionSnippet))
            {
                var lastFunctionLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains(Constants.NodeSetFunctioncContent.ReturnLine));
                if (lastFunctionLinePosition != -1)
                {
                    currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Empty);
                    currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.NodeSetFunctioncContent.FunctionSnippetPart5);
                    currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.NodeSetFunctioncContent.FunctionSnippetPart4);
                    currentFileContentLineByLine.Insert(lastFunctionLinePosition,string.Format(Constants.NodeSetFunctioncContent.FunctionSnippetPart3,functionName));
                    currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.NodeSetFunctioncContent.FunctionSnippetPart2);
                    currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Format(Constants.NodeSetFunctioncContent.FunctionSnippetPart1, functionName));
                }

                _fileSystem.WriteFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_nodeSetFunctions_c), currentFileContentLineByLine);
            }
        }

        
        private IEnumerable<string> ReadFileContent(Stream stream)
        {
            var fileContent = new List<string>();
            var sr = new StreamReader(stream);
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
            var pathToCreate = Path.Combine(srcDirectory, Constants.DirectoryName.InformationModels);
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