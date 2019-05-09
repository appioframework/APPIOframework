using System.Collections.Generic;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel
{
    public class NodesetGenerator : INodesetGenerator
    {
		private readonly string _projectName;
		private readonly string _modelFullName;
		private readonly string _typesFullName;

		private readonly IFileSystem _fileSystem;
		private readonly IModelValidator _modelValidator;

		private string _outputMessage;

		public NodesetGenerator(string projectName, string modelName, string typesName, IFileSystem fileSystem, IModelValidator modelValidator)
        {
			_projectName = projectName;
			_modelFullName = modelName;
			_typesFullName = typesName;
			_fileSystem = fileSystem;
			_modelValidator = modelValidator;
			_outputMessage = string.Empty;
		}

		public string GetOutputMessage()
		{
			return _outputMessage;
		}
        
		public bool GenerateTypesSourceCodeFiles()
		{
			// Verify types extension
			if (_fileSystem.GetExtension(_typesFullName) != Constants.FileExtension.ModelTypes)
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidFile, _typesFullName));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidFile, _projectName, _modelFullName, _typesFullName);
				return false;
			}

			// Verify if types file exists
			var typesPath = _fileSystem.CombinePaths(_projectName, Constants.DirectoryName.Models, _typesFullName);
			if(!_fileSystem.FileExists(typesPath))
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingFile, typesPath));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingFile, _projectName, _modelFullName, typesPath);
				return false;
			}
			
			// Create a directory for generated C code
			var srcDirectory = _fileSystem.CombinePaths(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			CreateNeededDirectories(srcDirectory);

			// Build types source file and target files directories
			var modelName = _fileSystem.GetFileNameWithoutExtension(_modelFullName);
			var typesSourceRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, _typesFullName);
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
				_outputMessage = string.Format(OutputText.GenerateInformationModelGenerateTypesFailure, _projectName, _modelFullName, _typesFullName);
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

		public bool GenerateNodesetSourceCodeFiles()
		{
			// Verify nodeset extension
			if (_fileSystem.GetExtension(_modelFullName) != Constants.FileExtension.InformationModel)
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsInvalidModelFile, _modelFullName));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureInvalidModel, _projectName, _modelFullName);
				return false;
			}

			// Verify if nodeset file exists
			var modelPath = _fileSystem.CombinePaths(_projectName, Constants.DirectoryName.Models, _modelFullName);
			if (!_fileSystem.FileExists(modelPath))
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetCompilerExecutableFailsMissingModelFile, _modelFullName));
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailureMissingModel, _projectName, _modelFullName, modelPath);
				return false;
			}

			// Validate nodeset
			if (!_modelValidator.Validate(modelPath, Resources.Resources.UANodeSetXsdFileName))
			{
				OppoLogger.Warn(string.Format(LoggingText.NodesetValidationFailure, _modelFullName));
				_outputMessage = string.Format(OutputText.NodesetValidationFailure, _modelFullName);
				return false;
			}
			
			// Create a directory for generated C code
			var srcDirectory = _fileSystem.CombinePaths(_projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp);
			CreateNeededDirectories(srcDirectory);

			// Build model source and target paths
			var modelName = _fileSystem.GetFileNameWithoutExtension(_modelFullName);
			var modelSourceRelativePath = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, _modelFullName);
			var modelTargetRelativePath = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName);

			// Build nodeset compiler script arguments
			var typesNameForScriptCall = _typesFullName == null ? Constants.ExecutableName.NodesetCompilerBasicTypes : (modelName + Constants.InformationModelsName.Types).ToUpper();
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
				_outputMessage = string.Format(OutputText.GenerateInformationModelFailure, _projectName, _modelFullName);
				return false;
			}


			return true;
		}
	}
}