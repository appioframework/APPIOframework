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

		private string _outputMessage;

		public NodesetGenerator(string projectName, string modelName, string typesName, IFileSystem fileSystem)
        {
			_projectName = projectName;
			_modelFullName = modelName;
			_typesFullName = typesName;
			_fileSystem = fileSystem;
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

			// Check if types file exists
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
			var typesSourceLocation = @"../../" + _fileSystem.CombinePaths(Constants.DirectoryName.Models, _typesFullName);
			var typesTargetLocation = _fileSystem.CombinePaths(Constants.DirectoryName.InformationModels, modelName.ToLower());

			// Build nodeset compiler python script arguments
			var generatedTypesArgs = Constants.ExecutableName.GenerateDatatypesScriptPath + string.Format(Constants.ExecutableName.GenerateDatatypesTypeBsd, typesSourceLocation) + " " + typesTargetLocation + Constants.InformationModelsName.Types;
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
	}
}