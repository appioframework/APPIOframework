using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

			if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains) || !_fileSystem.DirectoryExists(opcuaAppName))
			{
				OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
				outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// -s flag (temporary solution for now -> needs bigger design changes)
			if (pathFlag == Constants.ImportInformationModelCommandArguments.Sample || pathFlag == Constants.ImportInformationModelCommandArguments.VerboseSample)
			{
				var modelsDir = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);

				var nodesetContent = _fileSystem.LoadTemplateFile(Resources.Resources.SampleInformationModelFileName);
				var nodesetFilePath = _fileSystem.CombinePaths(modelsDir, Constants.FileName.SampleInformationModelFile);
				_fileSystem.CreateFile(nodesetFilePath, nodesetContent);

				var typesContent = _fileSystem.LoadTemplateFile(Resources.Resources.SampleInformationModelTypesFileName);
				var typesFilePath = _fileSystem.CombinePaths(modelsDir, Constants.FileName.SampleInformationModelTypesFile);
				_fileSystem.CreateFile(typesFilePath, typesContent);

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

			// here I should check if model is part of oppoproj
			// check if opcuaapp alrady has models with this name (uri later)
			var oppoprojFilePath = _fileSystem.CombinePaths(opcuaAppName, opcuaAppName + Constants.FileExtension.OppoProject);
			var opcuaappData = Deserialize.Opcuaapp(oppoprojFilePath, _fileSystem);
			if (opcuaappData == null)
			{
				OppoLogger.Warn(LoggingText.ImportInforamtionModelCommandFailureCannotReadOppoprojFile);
				outputMessages.Add(OutputText.ImportInforamtionModelCommandFailureCannotReadOppoprojFile, string.Empty);
				return new CommandResult(false, outputMessages);
			}
			if (opcuaappData.Type == Constants.ApplicationType.Client)
			{
				OppoLogger.Warn(LoggingText.ImportInformationModelCommandOpcuaappIsAClient);
				outputMessages.Add(OutputText.ImportInformationModelCommandOpcuaappIsAClient, string.Empty);
				return new CommandResult(false, outputMessages);
			}
			
			// here I should add the information to oppoproj file
			var modelData = new ModelData();
			modelData.Name = _fileSystem.GetFileName(modelFileName);
			modelData.Uri = string.Empty;
			modelData.Types = string.Empty;
			modelData.NamespaceVariable = "ns_" + _fileSystem.GetFileNameWithoutExtension(modelFileName);

			if(opcuaappData.Type == Constants.ApplicationType.Server)
				(opcuaappData as OpcuaServerApp).Models.Add(modelData);
			else if (opcuaappData.Type == Constants.ApplicationType.ClientServer)
				(opcuaappData as OpcuaClientServerApp).Models.Add(modelData);
			
			var oppoprojNewContent = JsonConvert.SerializeObject(opcuaappData, Formatting.Indented);

			_fileSystem.WriteFile(oppoprojFilePath, new List<string> { oppoprojNewContent });

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