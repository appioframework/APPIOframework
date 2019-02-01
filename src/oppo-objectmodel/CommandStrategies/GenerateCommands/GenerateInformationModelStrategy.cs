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

        private struct ArgumentsCheckMessages
        {
            public string outputMessage;
            public string loggerMessage;
        }

        private struct RequiredFiles
        {
            public bool typesRequired;
            public string typesFullName;
            public bool modelRequired;
            public string modelFullName;
            public string modelRequiredTypes;
        }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            // encapsulate input parameters into variables with meaningful names
            var inputParamsList = inputParams.ToList();

            var nameFlag                = inputParamsList.ElementAtOrDefault(0);
            var opcuaAppName            = inputParamsList.ElementAtOrDefault(1);
            var modelFlag               = inputParamsList.ElementAtOrDefault(2);
            var modelFullName           = inputParamsList.ElementAtOrDefault(3);
            var requiredFile1Flag       = inputParamsList.ElementAtOrDefault(4);
            var requiredFile1FullName   = inputParamsList.ElementAtOrDefault(5);
            var requiredFile2Flag       = inputParamsList.ElementAtOrDefault(6);
            var requiredFile2FullName   = inputParamsList.ElementAtOrDefault(7);

            var outputMessages = new MessageLines();
            var argumentsCheckMesseges = new ArgumentsCheckMessages();

            // validate project flag and name
            if (!ValidateProjectName(ref argumentsCheckMesseges, nameFlag, opcuaAppName, modelFullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // validate model flag and file
            if (!ValidateModelFile(ref argumentsCheckMesseges, opcuaAppName, modelFlag, modelFullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // validate required file 1 and 2 flags
            var requiredFiles = new RequiredFiles();
            if (!ValidateRequiredFile1(ref argumentsCheckMesseges, ref requiredFiles, opcuaAppName, modelFullName, requiredFile1Flag, requiredFile1FullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }
            if (!ValidateRequiredFile2(ref argumentsCheckMesseges, ref requiredFiles, opcuaAppName, modelFullName, requiredFile2Flag, requiredFile2FullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // validate types file
            if (!ValidateTypesFile(ref argumentsCheckMesseges, requiredFiles, opcuaAppName, modelFullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // validate required model file
            if (!ValidateRequiredModelFile(ref argumentsCheckMesseges, requiredFiles, opcuaAppName, modelFullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }
            
            // prepare source location ./src/server 
            var srcDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);

            // create inside src/server the information-models directory if it is not already created
            CreateNeededDirectories(srcDirectory);

            // execute generate datatypes python script if external types are required
            var modelName = System.IO.Path.GetFileNameWithoutExtension(modelFullName);
            if (requiredFiles.typesRequired)
            {
                var typesSourceLocation = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, requiredFiles.typesFullName);
                var typesTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower());

                var generatedTypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath + string.Format(Constants.ExecutableName.GenerateDatatypesTypeBsd, typesSourceLocation) + " " + typesTargetLocation;
                var generatedTypesResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, generatedTypesArgs);
                if (!generatedTypesResult)
                {
                    OppoLogger.Warn(LoggingText.GeneratedTypesExecutableFails);
                    outputMessages.Add(string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, opcuaAppName, modelFullName, requiredFiles.typesFullName), string.Empty);
                    return new CommandResult(false, outputMessages);
                }
            }

            // prepare model paths
            var modelTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName);
            var sourceModelRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, modelFullName);

            // prepare nodeset compiler call arguments
            var nodesetCompilerArgs = BuildNodesetCompilerArgs(requiredFiles, opcuaAppName, modelName, sourceModelRelativePath, modelTargetLocation);

            // execute nodeset compiler python script
            var nodesetResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs);
            if (!nodesetResult)
            {
                OppoLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailure, opcuaAppName, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // adjust models.c file with new libraries
            AdjustModelsTemplate(srcDirectory, modelName);
            if (requiredFiles.typesRequired)
            {
                AdjustModelsTemplate(srcDirectory, modelName.ToLower() + Constants.ModelsCContent._generated);
            }

            // adjust nodeSetFunctions.c with new functions
            AdjustNodeSetFunctionsTemplate(srcDirectory, modelName);

            outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, opcuaAppName, modelFullName), string.Empty);
            OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
            return new CommandResult(true, outputMessages);           
        }

        private bool ValidateProjectName(ref ArgumentsCheckMessages messages, string nameFlag, string opcuaAppName, string modelFullName)
        {
            // check project name flag
            if (nameFlag != Constants.GenerateInformationModeCommandArguments.Name && nameFlag != Constants.GenerateInformationModeCommandArguments.VerboseName)
            {
                messages.loggerMessage = string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, nameFlag);
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, nameFlag);
                return false;
            }

            // check if project exists
            if (string.IsNullOrEmpty(opcuaAppName))
            {
                messages.loggerMessage = LoggingText.GenerateInformationModelFailureEmptyOpcuaAppName;
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureEmptyOpcuaAppName, opcuaAppName, modelFullName);
                return false;
            }

            return true;
        }

        private bool ValidateModelFile(ref ArgumentsCheckMessages messages, string opcuaAppName, string modelFlag, string modelFullName)
        {
            // check model flag
            if (modelFlag != Constants.GenerateInformationModeCommandArguments.Model && modelFlag != Constants.GenerateInformationModeCommandArguments.VerboseModel)
            {
                messages.loggerMessage = string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, modelFlag);
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, modelFlag);
                return false;
            }

            // check if model file exists
            var calculatedModelFilePath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, modelFullName);
            if (!_fileSystem.FileExists(calculatedModelFilePath))
            {
                messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, calculatedModelFilePath);
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingModel, opcuaAppName, modelFullName, calculatedModelFilePath);
                return false;
            }

            // check if model file is an *.xml file
            var modelFileExtension = _fileSystem.GetExtension(modelFullName);
            if (modelFileExtension != Constants.FileExtension.InformationModel)
            {
                messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, modelFullName);
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidModel, opcuaAppName, modelFullName, modelFileExtension);
                return false;
            }

            // validate model
            if (!_modelValidator.Validate(calculatedModelFilePath, Resources.Resources.UANodeSetXsdFileName))
            {
                messages.loggerMessage = string.Format(LoggingText.GenerateInformationModelFailureValidatingModel, modelFullName);
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureValidatingModel, modelFullName);
                return false;
            }
            
            return true;
        }

        private bool ValidateRequiredFile1(ref ArgumentsCheckMessages messages, ref RequiredFiles requiredFiles, string opcuaAppName, string modelFullName, string requiredFile1Flag, string requiredFile1FullName)
        {
            //check types / required model flags
            if (requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.Types || requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.VerboseTypes)
            {
                requiredFiles.typesRequired = true;
                requiredFiles.typesFullName = requiredFile1FullName;
                requiredFiles.modelRequired = false;
                requiredFiles.modelFullName = string.Empty;
            }
            else if (requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.RequiredModel || requiredFile1Flag == Constants.GenerateInformationModeCommandArguments.VerboseRequiredModel)
            {
                requiredFiles.typesRequired = false;
                requiredFiles.typesFullName = string.Empty;
                requiredFiles.modelRequired = true;
                requiredFiles.modelFullName = requiredFile1FullName;
            }
            else if (!string.IsNullOrEmpty(requiredFile1Flag))
            {
                messages.loggerMessage = string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, requiredFile1Flag);
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, requiredFile1Flag);
                return false;
            }

            return true;
        }

        private bool ValidateTypesFile(ref ArgumentsCheckMessages messages, RequiredFiles requiredFiles, string opcuaAppName, string modelFullName)
        { 
            // proceed only if user defined required extra types
            if (requiredFiles.typesRequired)
            {
                // check if types file exists
                var calculatedRequiredTypesPath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredFiles.typesFullName);
                if (!_fileSystem.FileExists(calculatedRequiredTypesPath))
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, calculatedRequiredTypesPath);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingFile, opcuaAppName, modelFullName, calculatedRequiredTypesPath);
                    return false;
                }

                // check if types file is a *.bsd
                var requiredTypesFileExtension = _fileSystem.GetExtension(requiredFiles.typesFullName);
                if (requiredTypesFileExtension != Constants.FileExtension.ModelTypes)
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, requiredFiles.typesFullName);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidFile, opcuaAppName, modelFullName, requiredFiles.typesFullName);
                    return false;
                }
            }
            
            return true;
        }

        private bool ValidateRequiredFile2(ref ArgumentsCheckMessages messages, ref RequiredFiles requiredFiles, string opcuaAppName, string modelFullName, string requiredFile2Flag, string requiredFile2FullName)
        {
            //check types / required model flags
            if (requiredFile2Flag == Constants.GenerateInformationModeCommandArguments.RequiredModel || requiredFile2Flag == Constants.GenerateInformationModeCommandArguments.VerboseRequiredModel)
            {
                requiredFiles.modelRequired = true;
                requiredFiles.modelFullName = requiredFile2FullName;
            }
            else if (!string.IsNullOrEmpty(requiredFile2Flag))
            {
                messages.loggerMessage = string.Format(LoggingText.GenerateInformationModelFailureUnknownParam, requiredFile2Flag);
                messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureUnknownParam, opcuaAppName, modelFullName, requiredFile2Flag);
                return false;
            }

            return true;
        }

        private bool ValidateRequiredModelFile(ref ArgumentsCheckMessages messages, RequiredFiles requiredFiles, string opcuaAppName, string modelFullName)
        { 
            if (requiredFiles.modelRequired)
            {
                // check if required model file exists
                var calculatedRequiredModelPath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredFiles.modelFullName);
                if (!_fileSystem.FileExists(calculatedRequiredModelPath))
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, calculatedRequiredModelPath);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingFile, opcuaAppName, modelFullName, calculatedRequiredModelPath);
                    return false;
                }

                // check if required model file is a *.xml
                var requiredModelFileExtension = _fileSystem.GetExtension(requiredFiles.modelFullName);
                if (requiredModelFileExtension != Constants.FileExtension.InformationModel)
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, requiredFiles.modelFullName);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidFile, opcuaAppName, modelFullName, requiredFiles.modelFullName);
                    return false;
                }
            }

            return true;
        }

        private string BuildNodesetCompilerArgs(RequiredFiles requiredFiles, string opcuaAppName, string typesName, string modelSourceLocation, string modelTargetLocation)
        {
            var outputString = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);
            if (requiredFiles.modelRequired)
            {
                var requiredModelPath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredFiles.modelFullName);
                if (CheckIfModelRequiresTypes(requiredModelPath))
                {
                    var requiredModelName = _fileSystem.GetFileNameWithoutExtension(requiredFiles.modelFullName);
                    outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, requiredModelName.ToUpper());
                }
                else
                {
                    outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);
                }
            }
            if (requiredFiles.typesRequired)
            {
                outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, typesName.ToUpper());
            }
            else
            {
                outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);
            }
            outputString += string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset);
            if (requiredFiles.modelRequired)
            {
                var requiredModelSourceLocation = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, requiredFiles.modelFullName);
                outputString += string.Format(Constants.ExecutableName.NodesetCompilerExisting, requiredModelSourceLocation);
            }
            outputString += string.Format(Constants.ExecutableName.NodesetCompilerXml, modelSourceLocation, modelTargetLocation);

            return outputString;
        }

        private bool CheckIfModelRequiresTypes(string modelPath)
        {
            var modelFileStream = _fileSystem.ReadFile(modelPath);
            var currentFileContentLineByLine = ReadFileContent(modelFileStream).ToList();

            foreach(var line in currentFileContentLineByLine)
            {
                if(line.Contains(Constants.definitionXmlElement))
                {
                    modelFileStream.Close();
                    modelFileStream.Dispose();
                    return true;
                }
            }

            modelFileStream.Close();
            modelFileStream.Dispose();
            return false;
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