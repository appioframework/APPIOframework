using System.Collections.Generic;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel
{
    public class NodesetGenerator : INodesetGenerator
    {
		private readonly IFileSystem _fileSystem;
		private readonly IModelValidator _modelValidator;

		private string _outputMessage;
		
		public NodesetGenerator(IFileSystem fileSystem, IModelValidator modelValidator)
        {
			_fileSystem = fileSystem;
			_modelValidator = modelValidator;
			_outputMessage = string.Empty;
		}

		public string GetOutputMessage()
		{
			return _outputMessage;
		}

		public bool GenerateTypesSourceCodeFiles(string projectName, IModelData modelData)
		{
			// Verify if model has types
			if(string.IsNullOrEmpty(modelData.Types))
			{
				return true;
			}

			// Verify types extension
			if (_fileSystem.GetExtension(modelData.Types) != Constants.FileExtension.ModelTypes)
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, modelData.Types));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidFile, projectName, modelData.Name, modelData.Types);
				return false;
			}

			// Verify if types file exists
			var typesPath = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Models, modelData.Types);
			if(!_fileSystem.FileExists(typesPath))
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, typesPath));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingFile, projectName, modelData.Name, typesPath);
				return false;
			}
			
			// Create a directory for generated C code
			var srcDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			CreateNeededDirectories(srcDirectory);

			// Build types source file and target files directories
			var modelName = _fileSystem.GetFileNameWithoutExtension(modelData.Name);
			var typesSourceRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, modelData.Types);
			var typesTargetRelativePath = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower());

			// Build types generation script arguments
			var generatedTypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath +
										string.Format(Constants.ExecutableName.GenerateDatatypesTypeBsd, typesSourceRelativePath) +
										" " +
										typesTargetRelativePath +
										Constants.InformationModelsName.Types;

			// Execute types generation script call
			var generatedTypesResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, generatedTypesArgs);
			if (!generatedTypesResult)
			{
				OppoLogger.Warn(LoggingText.GeneratedTypesExecutableFails);
				_outputMessage = string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, projectName, modelData.Name, modelData.Types);
				return false;
			}

			return true;
		}
		
		private void CreateNeededDirectories(string srcDirectory)
		{
			var pathToCreate = _fileSystem.CombinePaths(srcDirectory, Constants.DirectoryName.InformationModels);
			if (!_fileSystem.DirectoryExists(pathToCreate))
			{
				_fileSystem.CreateDirectory(pathToCreate);
			}
		}

		public bool GenerateNodesetSourceCodeFiles(string projectName, IModelData modelData)
		{
			// Verify if nodeset file name is not empty
			if (string.IsNullOrEmpty(modelData.Name))
			{
				OppoLogger.Warn(LoggingText.GenerateInformationModelFailureEmptyModelName);
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureEmptyModelName, projectName);
				return false;
			}

			// Verify nodeset extension
			if (_fileSystem.GetExtension(modelData.Name) != Constants.FileExtension.InformationModel)
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, modelData.Name));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidModel, projectName, modelData.Name);
				return false;
			}

			// Verify if nodeset file exists
			var modelPath = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Models, modelData.Name);
			if (!_fileSystem.FileExists(modelPath))
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, modelData.Name));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingModel, projectName, modelData.Name, modelPath);
				return false;
			}

			// Validate nodeset
			if (!_modelValidator.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName))
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetValidationFailure, modelData.Name));
				_outputMessage = string.Format(OutputText.NodesetValidationFailure, modelData.Name);
				return false;
			}
			
			// Create a directory for generated C code
			var srcDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			CreateNeededDirectories(srcDirectory);

			// Build model source and target paths
			var modelName = _fileSystem.GetFileNameWithoutExtension(modelData.Name);
			var modelSourceRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, modelData.Name);
			var modelTargetRelativePath = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName);

			// Build nodeset compiler script arguments
			var typesNameForScriptCall = string.IsNullOrEmpty(modelData.Types) ? Constants.ExecutableName.NodesetCompilerBasicTypes : (modelName + Constants.InformationModelsName.Types).ToUpper();
			var nodesetCompilerArgs = Constants.ExecutableName.NodesetCompilerCompilerPath +
										Constants.ExecutableName.NodesetCompilerInternalHeaders +
										string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, Constants.ExecutableName.NodesetCompilerBasicTypes) +
										string.Format(Constants.ExecutableName.NodesetCompilerTypesArray, typesNameForScriptCall) +
										string.Format(Constants.ExecutableName.NodesetCompilerExisting, Constants.ExecutableName.NodesetCompilerBasicNodeset) +
										string.Format(Constants.ExecutableName.NodesetCompilerXml, modelSourceRelativePath, modelTargetRelativePath);

			// Execute nodeset compiler call
			var nodesetResult = _fileSystem.CallExecutable(Constants.ExecutableName.PythonScript, srcDirectory, nodesetCompilerArgs);
			if (!nodesetResult)
			{
				OppoLogger.Warn(LoggingText.NodesetCompilerExecutableFails);
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailure, projectName, modelData.Name);
				return false;
			}


			return true;
		}
	}
}