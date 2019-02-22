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
			var nameFlag		 = inputParamsArray.ElementAtOrDefault(0);
			var opcuaAppName	 = inputParamsArray.ElementAtOrDefault(1);
			var typeFlag		 = inputParamsArray.ElementAtOrDefault(2);
			var applicationType	 = inputParamsArray.ElementAtOrDefault(3);

			applicationType = string.IsNullOrEmpty(typeFlag) ? Constants.ApplicationType.ClientServer : applicationType;
			
			var outputMessages = new MessageLines();

			// validate opcuaapp name flag
			if (nameFlag != Constants.NewOpcuaAppCommandArguments.Name && nameFlag != Constants.NewOpcuaAppCommandArguments.VerboseName)
			{
				OppoLogger.Warn(LoggingText.UnknownNewOpcuaappCommandParam);
				outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, nameFlag), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// validate opcuaapp name
			if (string.IsNullOrEmpty(opcuaAppName) || _fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains) || _fileSystem.GetInvalidPathChars().Any(opcuaAppName.Contains))
			{
				OppoLogger.Warn(LoggingText.InvalidOpcuaappName);
				outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailureInvalidProjectName, opcuaAppName), string.Empty);
				return new CommandResult(false, outputMessages);
			}

			if (!string.IsNullOrEmpty(typeFlag))
			{
				// validate opcuaapp type flag
				if (typeFlag != Constants.NewOpcuaAppCommandArguments.Type && typeFlag != Constants.NewOpcuaAppCommandArguments.VerboseType)
				{
					OppoLogger.Warn(LoggingText.UnknownNewOpcuaappCommandParam);
					outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, typeFlag), string.Empty);
					return new CommandResult(false, outputMessages);
				}

				// validate opcuaapp type
				else if (applicationType != Constants.ApplicationType.Client && applicationType != Constants.ApplicationType.Server && applicationType != Constants.ApplicationType.ClientServer)
				{
					OppoLogger.Warn(LoggingText.InvalidOpcuaappType);
					outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailureUnknownProjectType, applicationType), string.Empty);
					return new CommandResult(false, outputMessages);
				}
			}

			// combine project file paths
			var projectFilePath = _fileSystem.CombinePaths(opcuaAppName, $"{opcuaAppName}{Constants.FileExtension.OppoProject}");
			var sourceDirectory = _fileSystem.CombinePaths(opcuaAppName, Constants.DirectoryName.SourceCode);
			var mesonFilePath = _fileSystem.CombinePaths(opcuaAppName, Constants.FileName.SourceCode_meson_build);

			// create project directories
			_fileSystem.CreateDirectory(opcuaAppName);
			_fileSystem.CreateDirectory(sourceDirectory);
			
			IOpcuaapp opcuaapp = null;

			// deploy files for opcuaapp Client
			if (applicationType == Constants.ApplicationType.Client)
			{
				opcuaapp = new OpcuaClientApp(opcuaAppName);
				_fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientType_build));

				DeployTemplateOpcuaClientSourceFiles(sourceDirectory);
			}
			// deploy files for opcuaapp Server
			else if (applicationType == Constants.ApplicationType.Server)
			{
				opcuaapp = new OpcuaServerApp(opcuaAppName, string.Empty);
				_fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ServerType_build));
				
				CreateModelsDirectory(opcuaAppName);
				DeployTemplateOpcuaServerSourceFiles(sourceDirectory);
			}
			// deploy files for opcuaapp ClientServer
			else if (applicationType == Constants.ApplicationType.ClientServer)
			{
				opcuaapp = new OpcuaClientServerApp(opcuaAppName, string.Empty);
				_fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ClientServerType_build));

				CreateModelsDirectory(opcuaAppName);
				DeployTemplateOpcuaClientSourceFiles(sourceDirectory);
				DeployTemplateOpcuaServerSourceFiles(sourceDirectory);
			}
			
			// create *.oppoproj file
			var opcuaappAsJson = JsonConvert.SerializeObject(opcuaapp, Formatting.Indented);
			_fileSystem.CreateFile(projectFilePath, opcuaappAsJson);

			OppoLogger.Info(string.Format(LoggingText.NewOpcuaappCommandSuccess, opcuaAppName));
			outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandSuccess, opcuaAppName), string.Empty);
			return new CommandResult(true, outputMessages);
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