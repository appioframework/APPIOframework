using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
	public class NewOpcuaAppCommandStrategy : ICommand<NewStrategy>
	{
		private readonly IFileSystem _fileSystem;

		public NewOpcuaAppCommandStrategy(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public string Name => Constants.NewCommandName.OpcuaApp;

		public CommandResult Execute(IEnumerable<string> inputParams)
		{
			var inputParamsArray = inputParams.ToArray();
			var nameFlag = inputParamsArray.ElementAtOrDefault(0);
			var opcuaAppName = inputParamsArray.ElementAtOrDefault(1);
			var typeFlag = inputParamsArray.ElementAtOrDefault(2);
			var applicationType = inputParamsArray.ElementAtOrDefault(3);

			applicationType = string.IsNullOrEmpty(applicationType) ? "ClientServer" : applicationType;
			
			var outputMessages = new MessageLines();

			if (nameFlag != Constants.NewOpcuaAppCommandArguments.Name && nameFlag != Constants.NewOpcuaAppCommandArguments.VerboseName)
			{
				OppoLogger.Warn(LoggingText.UnknownNewOpcuaappCommandParam);
				outputMessages.Add(OutputText.NewOpcuaappCommandFailureUnknownParam, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			if (string.IsNullOrEmpty(opcuaAppName))
			{
				OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
				outputMessages.Add(OutputText.NewOpcuaappCommandFailureUnknownParam, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains))
			{
				OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
				outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailure, opcuaAppName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			if (_fileSystem.GetInvalidPathChars().Any(opcuaAppName.Contains))
			{
				OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
				outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailure, opcuaAppName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// chceck type flag
			if (!string.IsNullOrEmpty(typeFlag))
			{
				if (typeFlag != "-t" && typeFlag != "--type")
				{
					OppoLogger.Warn(LoggingText.UnknownNewOpcuaappCommandParam);
					outputMessages.Add(string.Format("Unknown command parameter '{0}'!", typeFlag), string.Empty);
					return new CommandResult(false, outputMessages);
				}
				else if (applicationType != "ClientServer" && applicationType != "Client" && applicationType != "Server")
				{
					OppoLogger.Warn("Unknown opcua application type!");
					outputMessages.Add(string.Format("Unknown opcua application type '{0}'!", applicationType), string.Empty);
					return new CommandResult(false, outputMessages);
				}
			}

			// deploy files depends on chosen type
			DeployTemplateOpcuaApp(opcuaAppName, applicationType);

			var sourceDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode);
			_fileSystem.CreateDirectory(sourceDirectory);

			if (applicationType == "Client")
            {
				DeployTemplateOpcuaClientSourceFiles(sourceDirectory);

				OppoLogger.Info(string.Format("An opcuaapp '{0}' of Client type was successfully created!", opcuaAppName));
				outputMessages.Add(string.Format("An opcuaapp '{0}' of Client type was successfully created!", opcuaAppName), string.Empty);
			}
			else if (applicationType == "Server")
            {
				CreateModelsDirectory(opcuaAppName);
				DeployTemplateOpcuaServerSourceFiles(sourceDirectory);

				OppoLogger.Info(string.Format("An opcuaapp '{0}' of Server type was successfully created!", opcuaAppName));
				outputMessages.Add(string.Format("An opcuaapp '{0}' of Server type was successfully created!", opcuaAppName), string.Empty);
			}
			else
			{
				CreateModelsDirectory(opcuaAppName);
				DeployTemplateOpcuaClientSourceFiles(sourceDirectory);
				DeployTemplateOpcuaServerSourceFiles(sourceDirectory);

				OppoLogger.Info(string.Format("An opcuaapp '{0}' of ClientServer type was successfully created!", opcuaAppName));
				outputMessages.Add(string.Format("An opcuaapp '{0}' of ClientServer type was successfully created!", opcuaAppName), string.Empty);
			}

			return new CommandResult(true, outputMessages);
		}

		private void DeployTemplateOpcuaApp(string opcuaAppName, string applicatonType)
		{
			_fileSystem.CreateDirectory(opcuaAppName);
			var projectFilePath = _fileSystem.CombinePaths(opcuaAppName, $"{opcuaAppName}{Constants.FileExtension.OppoProject}");
            var mesonFilePath = _fileSystem.CombinePaths(opcuaAppName, Constants.FileName.SourceCode_meson_build);

            IOpcuaapp opcuaapp = null;
            if (applicatonType == "Server")
            {
                opcuaapp = new OpcuaServerApp(opcuaAppName, string.Empty);
                _fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ServerType_build));
            }

            if (applicatonType == "Client")
            {
                opcuaapp = new OpcuaClientApp(opcuaAppName);
                _fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientType_build));
            }

            if (applicatonType == "ClientServer")
            {
                opcuaapp = new OpcuaClientServerApp(opcuaAppName, string.Empty);
                _fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientServerType_build));
            }
            
			var opcuaappAsJson = JsonConvert.SerializeObject(opcuaapp, Formatting.Indented);
			_fileSystem.CreateFile(projectFilePath, opcuaappAsJson);
		}

		private void CreateModelsDirectory(string opcuaAppName)
		{
			var modelsDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.Models);
			_fileSystem.CreateDirectory(modelsDirectory);
		}

		private void DeployTemplateOpcuaClientSourceFiles(string sourceDirectory)
		{
			var appSourceDirectory = _fileSystem.CombinePaths(sourceDirectory, Constants.DirectoryName.ClientApp);
			_fileSystem.CreateDirectory(appSourceDirectory);
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_client_c));
		}

		private void DeployTemplateOpcuaServerSourceFiles(string sourceDirectory)
		{
			var appSourceDirectory = _fileSystem.CombinePaths(sourceDirectory, Constants.DirectoryName.ServerApp);
			_fileSystem.CreateDirectory(appSourceDirectory);
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_server_c));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_meson_build), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_server_build));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_loadInformationModels_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_loadInformationModels_server_c));
		}

		public string GetHelpText()
		{
			return string.Empty;
		}
	}
}