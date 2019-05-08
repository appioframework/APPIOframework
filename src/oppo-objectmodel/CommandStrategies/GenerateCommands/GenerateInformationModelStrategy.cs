using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Oppo.ObjectModel.CommandStrategies.GenerateCommands
{
    public class GenerateInformationModelStrategy : ICommand<GenerateStrategy>
    {
        private enum ParamId {AppName, ModelFullName, TypesFullName, RequiredModelFullName}

        private readonly ParameterResolver<ParamId> _resolver;
        private readonly IFileSystem _fileSystem;
        private readonly IModelValidator _modelValidator;

        public GenerateInformationModelStrategy(string commandName, IFileSystem fileSystem, IModelValidator modelValidator)
        {
            _fileSystem = fileSystem;
            _modelValidator = modelValidator;
            Name = commandName;
            _resolver = new ParameterResolver<ParamId>(Constants.CommandName.Generate + " " + Name, new []
            {
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.AppName,
                    Short = Constants.GenerateInformationModeCommandArguments.Name,
                    Verbose = Constants.GenerateInformationModeCommandArguments.VerboseName
                },
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.ModelFullName,
                    Short = Constants.GenerateInformationModeCommandArguments.Model,
                    Verbose = Constants.GenerateInformationModeCommandArguments.VerboseModel
                },
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.TypesFullName,
                    Short = Constants.GenerateInformationModeCommandArguments.Types,
                    Verbose = Constants.GenerateInformationModeCommandArguments.VerboseTypes,
                    Default = string.Empty
                },
                new StringParameterSpecification<ParamId>
                {
                    Identifier = ParamId.RequiredModelFullName,
                    Short = Constants.GenerateInformationModeCommandArguments.RequiredModel,
                    Verbose = Constants.GenerateInformationModeCommandArguments.VerboseRequiredModel,
                    Default = string.Empty
                }
            });
        }

        public string Name { get; private set; }

        private struct ArgumentsCheckMessages
        {
            public string outputMessage;
            public string loggerMessage;
        }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var (error, stringParams, _) = _resolver.ResolveParams(inputParams);
            
            if (error != null)
                return new CommandResult(false, new MessageLines{{error, string.Empty}});

            var opcuaAppName = stringParams[ParamId.AppName];
            var modelFullName = stringParams[ParamId.ModelFullName];
            var requiredFullName = stringParams[ParamId.RequiredModelFullName];
            var typesFullName = stringParams[ParamId.TypesFullName];

            var outputMessages = new MessageLines();
            var argumentsCheckMesseges = new ArgumentsCheckMessages();

            // validate model flag and file
            if (!ValidateModelFile(ref argumentsCheckMesseges, opcuaAppName, modelFullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // validate types file
            if (!ValidateTypesFile(ref argumentsCheckMesseges, typesFullName, opcuaAppName, modelFullName))
            {
                OppoLogger.Warn(argumentsCheckMesseges.loggerMessage);
                outputMessages.Add(argumentsCheckMesseges.outputMessage, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // validate required model file
            if (!ValidateRequiredModelFile(ref argumentsCheckMesseges, opcuaAppName, modelFullName, requiredFullName))
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
            if (typesFullName != string.Empty)
            {
                var typesSourceLocation = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, typesFullName);
                var typesTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower());

                var generatedTypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath + string.Format(Constants.ExecutableName.GenerateDatatypesTypeBsd, typesSourceLocation) + " " + typesTargetLocation + Constants.InformationModelsName.Types;
                var generatedTypesResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, generatedTypesArgs);
                if (!generatedTypesResult)
                {
                    OppoLogger.Warn(LoggingText.GeneratedTypesExecutableFails);
                    outputMessages.Add(string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, opcuaAppName, modelFullName, typesFullName), string.Empty);
                    return new CommandResult(false, outputMessages);
                }
            }

            // prepare model paths
            var modelTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName);
            var sourceModelRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, modelFullName);

            // prepare nodeset compiler call arguments
            var nodesetCompilerArgs = BuildNodesetCompilerArgs(opcuaAppName, modelName, typesFullName, requiredFullName, sourceModelRelativePath, modelTargetLocation);

            // execute nodeset compiler python script
            var nodesetResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs);
            if (!nodesetResult)
            {
                OppoLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
                outputMessages.Add(string.Format(OutputText.GenerateInformationModelFailure, opcuaAppName, modelFullName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

			// generate UA_Method callbacks
			CreateUAMethodCallbacks(srcDirectory, opcuaAppName, modelFullName);

			// adjust server meson.build file with new libraries
			AdjustServerMesonBuildTemplate(srcDirectory, modelName);
            if (typesFullName != string.Empty)
            {
                AdjustServerMesonBuildTemplate(srcDirectory, modelName.ToLower() + Constants.InformationModelsName.TypesGenerated);
            }

            // adjust loadInformationModels.c with new functions
            AdjustLoadInformationModelsTemplate(srcDirectory, modelName);

            outputMessages.Add(string.Format(OutputText.GenerateInformationModelSuccess, opcuaAppName, modelFullName), string.Empty);
            OppoLogger.Info(LoggingText.GenerateInformationModelSuccess);
            return new CommandResult(true, outputMessages);           
        }
        private bool ValidateModelFile(ref ArgumentsCheckMessages messages, string opcuaAppName, string modelFullName)
        {
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
                messages.loggerMessage = string.Format(LoggingText.NodesetValidationFailure, modelFullName);
                messages.outputMessage = string.Format(OutputText.NodesetValidationFailure, modelFullName);
                return false;
            }
            
            return true;
        }

        private bool ValidateTypesFile(ref ArgumentsCheckMessages messages, string typesFullName, string opcuaAppName, string modelFullName)
        { 
            // proceed only if user defined required extra types
            if (typesFullName != string.Empty)
            {
                // check if types file exists
                var calculatedRequiredTypesPath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, typesFullName);
                if (!_fileSystem.FileExists(calculatedRequiredTypesPath))
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, calculatedRequiredTypesPath);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingFile, opcuaAppName, modelFullName, calculatedRequiredTypesPath);
                    return false;
                }

                // check if types file is a *.bsd
                var requiredTypesFileExtension = _fileSystem.GetExtension(typesFullName);
                if (requiredTypesFileExtension != Constants.FileExtension.ModelTypes)
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, typesFullName);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidFile, opcuaAppName, modelFullName, typesFullName);
                    return false;
                }
            }
            
            return true;
        }

        private bool ValidateRequiredModelFile(ref ArgumentsCheckMessages messages, string opcuaAppName,
            string modelFullName, string requiredFullName)
        { 
            if (requiredFullName != string.Empty)
            {
                // check if required model file exists
                var calculatedRequiredModelPath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredFullName);
                if (!_fileSystem.FileExists(calculatedRequiredModelPath))
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, calculatedRequiredModelPath);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingFile, opcuaAppName, modelFullName, calculatedRequiredModelPath);
                    return false;
                }

                // check if required model file is a *.xml
                var requiredModelFileExtension = _fileSystem.GetExtension(requiredFullName);
                if (requiredModelFileExtension != Constants.FileExtension.InformationModel)
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, requiredFullName);
                    messages.outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidFile, opcuaAppName, modelFullName, requiredFullName);
                    return false;
                }

                // validate model
                if (!_modelValidator.Validate(calculatedRequiredModelPath, Resources.Resources.UANodeSetXsdFileName))
                {
                    messages.loggerMessage = string.Format(LoggingText.NodesetValidationFailure, requiredFullName);
                    messages.outputMessage = string.Format(OutputText.NodesetValidationFailure, requiredFullName);
                    return false;
                }
            }

            return true;
        }

        private string BuildNodesetCompilerArgs(string opcuaAppName, string typesName, string typesFullName, string requiredFullName, string modelSourceLocation,
            string modelTargetLocation)
        {
            // add nodeset compiler script path and basic ua types for basic ua nodeset
            var outputString = Constants.ExecutableName.NodesetCompilerCompilerPath + Constants.ExecutableName.NodesetCompilerInternalHeaders + string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);

            // add required model types
            if (requiredFullName != string.Empty)
            {
                var requiredModelPath = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, requiredFullName);
                if (CheckIfModelRequiresTypes(requiredModelPath))
                {
                    var requiredModelName = _fileSystem.GetFileNameWithoutExtension(requiredFullName);
                    outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, (requiredModelName + Constants.InformationModelsName.Types).ToUpper());
                }
                else
                {
                    outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);
                }
            }

            // add model extra types
            if (typesFullName != string.Empty)
            {
                outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, (typesName + Constants.InformationModelsName.Types).ToUpper());
            }
            else
            {
                outputString += string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes);
            }

            // add basic nodeset and required model files
            outputString += string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset);
            if (requiredFullName != string.Empty)
            {
                var requiredModelSourceLocation = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, requiredFullName);
                outputString += string.Format(Constants.ExecutableName.NodesetCompilerExisting, requiredModelSourceLocation);
            }

            // add information-model nodeset file
            outputString += string.Format(Constants.ExecutableName.NodesetCompilerXml, modelSourceLocation, modelTargetLocation);

            return outputString;
        }

        private bool CheckIfModelRequiresTypes(string modelPath)
        {
            // read content of information-model file
            var modelFileStream = _fileSystem.ReadFile(modelPath);
            var currentFileContentLineByLine = ReadFileContent(modelFileStream).ToList();

            // look for definition xml element which indicates that extra types are required
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
        /// meson.build stored in src/server template should files('fileName') line for-each generated information-model source code.
        /// Like for myModel.c, myModel.h -> "files('information-models/myModel.c'),".
        /// </summary>
        /// <param name="srcDirectory">Server source directory for current opcuaapp project.</param>
        /// <param name="fileNameToInclude">Generated file name without extension to be included.</param>
        private void AdjustServerMesonBuildTemplate(string srcDirectory, string fileNameToInclude)
        {
            var sourceFileSnippet = string.Format(Constants.InformationModelsName.FileSnippet, fileNameToInclude);

			using (var modelsFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_meson_build)))
			{
				var currentFileContentLineByLine = ReadFileContent(modelsFileStream);

				if (!currentFileContentLineByLine.Any(x => x.Contains(sourceFileSnippet)))
				{
					using (var sw = new StreamWriter(modelsFileStream))
					{
						foreach (var previousTextLine in currentFileContentLineByLine)
						{
							if (previousTextLine.Contains("]"))
							{
								sw.WriteLine(sourceFileSnippet);
							}

							sw.WriteLine(previousTextLine);
						}
					}
				}
			}
        }

        private void AdjustLoadInformationModelsTemplate(string srcDirectory, string functionName)
        {
            var functionSnippet = string.Format(Constants.LoadInformationModelsContent.FunctionSnippetPart1,functionName);

			var loadInformationModelsFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_loadInformationModels_c));
			var currentFileContentLineByLine = ReadFileContent(loadInformationModelsFileStream).ToList();

			loadInformationModelsFileStream.Close();
			loadInformationModelsFileStream.Dispose();

			if (!currentFileContentLineByLine.Any(x => x.Contains(functionSnippet.Substring(1))))
			{
				var lastFunctionLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains(Constants.LoadInformationModelsContent.ReturnLine));
				if (lastFunctionLinePosition != -1)
				{
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Empty);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.LoadInformationModelsContent.FunctionSnippetPart5);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.LoadInformationModelsContent.FunctionSnippetPart4);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Format(Constants.LoadInformationModelsContent.FunctionSnippetPart3, functionName));
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, Constants.LoadInformationModelsContent.FunctionSnippetPart2);
					currentFileContentLineByLine.Insert(lastFunctionLinePosition, string.Format(Constants.LoadInformationModelsContent.FunctionSnippetPart1, functionName));
				}

				_fileSystem.WriteFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_loadInformationModels_c), currentFileContentLineByLine);
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

		private struct UAMethod
		{
			public string BrowseName;
			public int NamespaceId;
			public int NodeId;
		}

		private void CreateUAMethodCallbacks(string srcDirectory, string opcuaAppName, string modelFullName)
		{
			// find all UA_Methods in generated nodeset
			List<UAMethod> uaMethodCollection = new List<UAMethod>();
			var nodesetFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models, modelFullName));
			using (XmlReader reader = XmlReader.Create(nodesetFileStream))
			{
				reader.MoveToContent();
				while (reader.Read())
				{
					if (reader.Name == Constants.UAMethodCallback.UAMethod && reader.IsStartElement() && reader.AttributeCount > 0)
					{
						var uaMethod = new UAMethod();

						uaMethod.BrowseName = reader.GetAttribute(Constants.UAMethodCallback.BrowseName);
						var idValues = reader.GetAttribute(Constants.UAMethodCallback.NodeId).Split(';');
						uaMethod.NamespaceId = int.Parse(string.Concat(idValues[0].Where(char.IsDigit)));
						uaMethod.NodeId = int.Parse(string.Concat(idValues[1].Where(char.IsDigit)));

						uaMethodCollection.Add(uaMethod);
					}
				}
			}

			// foreach found UA_Method generate callback function and call it in "addCallbacks" function
			List<string> currentFileContentLineByLine = null;
			using (var mainCallbacksFileStream = _fileSystem.ReadFile(_fileSystem.CombinePaths(srcDirectory, Constants.FileName.SourceCode_mainCallbacks_c)))
			{
				currentFileContentLineByLine = ReadFileContent(mainCallbacksFileStream).ToList();
			}
			foreach (var uaMethod in uaMethodCollection)
			{
				var lastUAMethodLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains(string.Format(Constants.UAMethodCallback.FunctionName, uaMethod.NamespaceId, uaMethod.NodeId)));
				if (lastUAMethodLinePosition == -1)
				{
					// add callback function
					var addCallbacksFunctionLinePosition = currentFileContentLineByLine.FindIndex(x => x.Contains(Constants.UAMethodCallback.AddCallbacks));
					if (addCallbacksFunctionLinePosition != -1)
					{
						currentFileContentLineByLine.Insert(addCallbacksFunctionLinePosition, string.Format(Constants.UAMethodCallback.FunctionBody, uaMethod.BrowseName, uaMethod.NamespaceId, uaMethod.NodeId));
					}

					// call callback function in addCallbacks function
					var addCallbacksReturnLinePosition = currentFileContentLineByLine.FindLastIndex(x => x.Contains(Constants.UAMethodCallback.ReturnLine));
					if (addCallbacksReturnLinePosition != -1)
					{
						currentFileContentLineByLine.Insert(addCallbacksReturnLinePosition, string.Format(Constants.UAMethodCallback.FunctionCall, uaMethod.BrowseName, uaMethod.NamespaceId, uaMethod.NodeId));
					}
				}
			}

			// write mainCallbacks.c file skipped for not due to problems with UAMethod namespace. Write command should be placed here.
		}

		public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.GenerateInformationModelCommandDescription;
        }
    }
}