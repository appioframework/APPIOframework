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

		private struct Messages
		{
			public string outputMessage;
			public string loggerMessage;
		}

		public string Name => Constants.NewCommandName.OpcuaApp;

		public CommandResult Execute(IEnumerable<string> inputParams)
		{
			var inputParamsArray = inputParams.ToArray();
			var nameFlag		 = inputParamsArray.ElementAtOrDefault(0);
			var opcuaAppName	 = inputParamsArray.ElementAtOrDefault(1);
			var typeFlag		 = inputParamsArray.ElementAtOrDefault(2);
			var applicationType	 = inputParamsArray.ElementAtOrDefault(3);
			var urlFlag			 = inputParamsArray.ElementAtOrDefault(4);
			var url				 = inputParamsArray.ElementAtOrDefault(5);
			var portFlag		 = inputParamsArray.ElementAtOrDefault(6);
			var port			 = inputParamsArray.ElementAtOrDefault(7);

			applicationType = string.IsNullOrEmpty(typeFlag) ? Constants.ApplicationType.ClientServer : applicationType;
			
			var outputMessages = new MessageLines();
			var messages = new Messages();

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

			// validate opcuaapp type
			if (!ValidateApplicationType(ref messages, typeFlag, applicationType, urlFlag, url, portFlag, port))
			{
				OppoLogger.Warn(messages.loggerMessage);
				outputMessages.Add(messages.outputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
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
				opcuaapp = new OpcuaServerApp(opcuaAppName, url, port);
				_fileSystem.CreateFile(mesonFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_ServerType_build));
				
				CreateModelsDirectory(opcuaAppName);
				DeployTemplateOpcuaServerSourceFiles(sourceDirectory);
			}
			// deploy files for opcuaapp ClientServer
			else if (applicationType == Constants.ApplicationType.ClientServer)
			{
				opcuaapp = new OpcuaClientServerApp(opcuaAppName, url, port);
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
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_globalVariables_h), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_globalVariables_client_h));
		}

		private void DeployTemplateOpcuaServerSourceFiles(string sourceDirectory)
		{
			var appSourceDirectory = _fileSystem.CombinePaths(sourceDirectory, Constants.DirectoryName.ServerApp);
			_fileSystem.CreateDirectory(appSourceDirectory);
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_main_server_c));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_meson_build), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_meson_server_build));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_loadInformationModels_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_loadInformationModels_server_c));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_constants_h), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_constants_server_h));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_mainCallbacks_c), _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName_mainCallbacks_c));
		}

		private bool ValidateApplicationType(ref Messages messages, string typeFlag, string applicationType, string urlFlag, string url, string portFlag, string port)
		{
			// validate opcuaapp type flag
			if (typeFlag != Constants.NewOpcuaAppCommandArguments.Type && typeFlag != Constants.NewOpcuaAppCommandArguments.VerboseType)
			{
				messages.loggerMessage = LoggingText.UnknownNewOpcuaappCommandParam;
				messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, typeFlag);
				return false;
			}
			// validate opcuaapp type
			else if (applicationType != Constants.ApplicationType.Client && applicationType != Constants.ApplicationType.Server && applicationType != Constants.ApplicationType.ClientServer)
			{
				messages.loggerMessage = LoggingText.InvalidOpcuaappType;
				messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureUnknownProjectType, applicationType);
				return false;
			}
			else if (applicationType == Constants.ApplicationType.Server || applicationType == Constants.ApplicationType.ClientServer)
			{
				// validate server url flag
				if (urlFlag != Constants.NewOpcuaAppCommandArguments.Url && urlFlag != Constants.NewOpcuaAppCommandArguments.VerboseUrl)
				{
					messages.loggerMessage = LoggingText.UnknownNewOpcuaappCommandParam;
					messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, urlFlag);
					return false;
				}
				// validate server url
				else if (ValidateUrl(url))
				{
					messages.loggerMessage = LoggingText.InvalidServerUrl;
					messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureInvalidServerUrl, url);
					return false;
				}

				// validate server port flag
				if (portFlag != Constants.NewOpcuaAppCommandArguments.Port && portFlag != Constants.NewOpcuaAppCommandArguments.VerbosePort)
				{
					messages.loggerMessage = LoggingText.UnknownNewOpcuaappCommandParam;
					messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureUnknownParam, portFlag);
					return false;
				}
				// validate server port
				else if (ValidatePort(port))
				{
					messages.loggerMessage = LoggingText.InvalidServerPort;
					messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureInvalidServerPort, port);
					return false;
				}
			}

			return true;
		}

		private bool ValidateUrl(string url)
		{
			return string.IsNullOrEmpty(url) || url.Any(x => char.IsWhiteSpace(x));
		}

		private bool ValidatePort(string port)
		{
			return string.IsNullOrEmpty(port) || !(port.All(x => char.IsDigit(x)) && Enumerable.Range(0, 65535).Contains(int.Parse(port)));
		}

		public string GetHelpText()
		{
			return string.Empty;
		}
	}
}