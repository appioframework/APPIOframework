using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.ImportCommands
{
    public class ImportInformationModelCommandStrategy : ICommand<ImportStrategy>
    {
        private readonly IFileSystem _fileSystem;
		private readonly MessageLines _outputMessages;

		public ImportInformationModelCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
			_outputMessages = new MessageLines();
		}

        public string Name => Constants.ImportInformationModelCommandName.InformationModel;

		private Dictionary<string, Type> dict = new Dictionary<string, Type>();

		public CommandResult Execute(IEnumerable<string> inputParams)
		{
			var inputParamsList = inputParams.ToList();
			var nameFlag = inputParamsList.ElementAtOrDefault(0);
			var opcuaAppName = inputParamsList.ElementAtOrDefault(1);
			var pathFlag = inputParams.ElementAtOrDefault(2);
			var modelPath = inputParamsList.ElementAtOrDefault(3);

			// opcuaapp name validation
			if (!ValidateOpcuaAppName(nameFlag, opcuaAppName))
			{
				return new CommandResult(false, _outputMessages);
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

				_outputMessages.Add(string.Format(OutputText.ImportSampleInformationModelSuccess, Constants.FileName.SampleInformationModelFile), string.Empty);
				OppoLogger.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, Constants.FileName.SampleInformationModelFile));
				return new CommandResult(true, _outputMessages);
			}

			// path flag validation
			if (!ValidateModel(pathFlag, modelPath))
			{
				return new CommandResult(false, _outputMessages);
			}
			var modelFileName = _fileSystem.GetFileName(modelPath);

			// here I should check if model is part of oppoproj
			// check if opcuaapp alrady has models with this name (uri later)
			var oppoprojFilePath = _fileSystem.CombinePaths(opcuaAppName, opcuaAppName + Constants.FileExtension.OppoProject);
			var opcuaappData = Deserialize.Opcuaapp(oppoprojFilePath, _fileSystem);
			if (opcuaappData == null)
			{
				OppoLogger.Warn(LoggingText.ImportInforamtionModelCommandFailureCannotReadOppoprojFile);
				_outputMessages.Add(OutputText.ImportInforamtionModelCommandFailureCannotReadOppoprojFile, string.Empty);
				return new CommandResult(false, _outputMessages);
			}
			if (opcuaappData.Type == Constants.ApplicationType.Client)
			{
				OppoLogger.Warn(LoggingText.ImportInformationModelCommandOpcuaappIsAClient);
				_outputMessages.Add(OutputText.ImportInformationModelCommandOpcuaappIsAClient, string.Empty);
				return new CommandResult(false, _outputMessages);
			}
			
			// here I should add the information to oppoproj file
			var modelData = new ModelData();
			modelData.Name = _fileSystem.GetFileName(modelFileName);
			modelData.Uri = GetNodesetUri(modelPath);
			modelData.Types = string.Empty;
			modelData.NamespaceVariable = "ns_" + _fileSystem.GetFileNameWithoutExtension(modelFileName);


			// check if oppoproj file already contains imported model
			if((opcuaappData as IOpcuaServerApp).Models.Any(x => x.Name == modelData.Name) || (opcuaappData as IOpcuaServerApp).Models.Any(x => x.Uri == modelData.Uri))
			{
				OppoLogger.Warn(LoggingText.ImportInforamtionModelCommandFailureModelDuplication);
				_outputMessages.Add(string.Format(OutputText.ImportInforamtionModelCommandFailureModelDuplication, opcuaAppName, modelFileName), string.Empty);
				return new CommandResult(false, _outputMessages);
			}
			
			(opcuaappData as IOpcuaServerApp).Models.Add(modelData);

			var oppoprojNewContent = JsonConvert.SerializeObject(opcuaappData, Newtonsoft.Json.Formatting.Indented);

			_fileSystem.WriteFile(oppoprojFilePath, new List<string> { oppoprojNewContent });

			var modelsDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);            
            var targetModelFilePath = _fileSystem.CombinePaths(modelsDirectory, modelFileName);
            _fileSystem.CopyFile(modelPath, targetModelFilePath);

            OppoLogger.Info(string.Format(LoggingText.ImportInforamtionModelCommandSuccess, modelPath));
			_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandSuccess, modelPath), string.Empty);
            return new CommandResult(true, _outputMessages);
        }

		private bool ValidateOpcuaAppName(string nameFlag, string opcuaAppName)
		{
			// opcuaapp name flag validation
			if (nameFlag != Constants.ImportInformationModelCommandArguments.Name && nameFlag != Constants.ImportInformationModelCommandArguments.VerboseName)
			{
				OppoLogger.Warn(LoggingText.UnknownImportInfomrationModelCommandParam);
				_outputMessages.Add(OutputText.ImportInformationModelCommandUnknownParamFailure, string.Empty);
				return false;
			}

			// opcuaapp name validation
			if (string.IsNullOrEmpty(opcuaAppName))
			{
				OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
				_outputMessages.Add(OutputText.ImportInformationModelCommandUnknownParamFailure, string.Empty);
				return false;
			}

			if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains) || !_fileSystem.DirectoryExists(opcuaAppName))
			{
				OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidOpcuaappName, opcuaAppName), string.Empty);
				return false;
			}

			return true;
		}

		private bool ValidateModel(string pathFlag, string modelPath)
		{
			// path flag validation
			if (pathFlag != Constants.ImportInformationModelCommandArguments.Path && pathFlag != Constants.ImportInformationModelCommandArguments.VerbosePath)
			{
				OppoLogger.Warn(LoggingText.UnknownImportInfomrationModelCommandParam);
				_outputMessages.Add(OutputText.ImportInformationModelCommandUnknownParamFailure, string.Empty);
				return false;
			}

			// model path validation
			if (string.IsNullOrEmpty(modelPath))
			{
				OppoLogger.Warn(LoggingText.InvalidInformationModelMissingModelFile);
				_outputMessages.Add(OutputText.ImportInformationModelCommandMissingModelPath, string.Empty);
				return false;
			}

			if (_fileSystem.GetInvalidPathChars().Any(modelPath.Contains))
			{
				OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelPath, modelPath));
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidModelPath, modelPath), string.Empty);
				return false;
			}

			if (!_fileSystem.FileExists(modelPath))
			{
				OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelNotExistingPath, modelPath));
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandNotExistingModelPath, modelPath), string.Empty);
				return false;
			}

			// model file name/extension validation
			var modelFileName = _fileSystem.GetFileName(modelPath);
			if (_fileSystem.GetExtension(modelPath) != Constants.FileExtension.InformationModel)
			{
				OppoLogger.Warn(string.Format(LoggingText.InvalidInformationModelExtension, modelFileName));
				_outputMessages.Add(string.Format(OutputText.ImportInformationModelCommandInvalidModelExtension, modelFileName), string.Empty);
				return false;
			}

			return true;
		}

		private string GetNodesetUri(string nodesetPath)
		{
			XmlDocument nodesetXml = new XmlDocument();
			using (var nodesetStream = _fileSystem.ReadFile(nodesetPath))
			{
				StreamReader reader = new StreamReader(nodesetStream);
				var xmlFileContent = reader.ReadToEnd();
				nodesetXml.LoadXml(xmlFileContent);
			}
			
			var nsmgr = new XmlNamespaceManager(nodesetXml.NameTable);
			nsmgr.AddNamespace("ns", "http://opcfoundation.org/UA/2011/03/UANodeSet.xsd");
			var uriNamespace = nodesetXml.SelectSingleNode("//ns:UANodeSet//ns:Models//ns:Model", nsmgr);
			return uriNamespace.Attributes["ModelUri"].Value;
		}

		public string GetHelpText()
        {
            return string.Empty;
        }
    }
}