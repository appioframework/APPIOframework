/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using Appio.Resources.text.logging;
using Appio.Resources.text.output;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appio.ObjectModel;
using Newtonsoft.Json;

namespace Appio.ObjectModel.CommandStrategies.NewCommands
{
	public class NewOpcuaAppCommandStrategy : ICommand<NewStrategy>
	{
		private readonly IFileSystem _fileSystem;
		private readonly ParameterResolver<ParamId> _resolver;
		private readonly AbstractCertificateGenerator _certificateGenerator;

		private enum ParamId {OpcuaAppName, ApplicationType, Url, Port, NoCert}
		
		public NewOpcuaAppCommandStrategy(IFileSystem fileSystem, AbstractCertificateGenerator certificateGenerator)
		{
			_fileSystem = fileSystem;
			_certificateGenerator = certificateGenerator;
			_resolver = new ParameterResolver<ParamId>(Constants.CommandName.New + " " + Name, new []
			{
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.OpcuaAppName,
					Short = Constants.NewCommandOptions.Name,
					Verbose = Constants.NewCommandOptions.VerboseName
				}, 
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.ApplicationType,
					Short = Constants.NewCommandOptions.Type,
					Verbose = Constants.NewCommandOptions.VerboseType,
					Default = Constants.ApplicationType.ClientServer
				},
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.Url,
					Short = Constants.NewCommandOptions.Url,
					Verbose = Constants.NewCommandOptions.VerboseUrl,
					Default = string.Empty
				},
				new StringParameterSpecification<ParamId>
				{
					Identifier = ParamId.Port,
					Short = Constants.NewCommandOptions.Port,
					Verbose = Constants.NewCommandOptions.VerbosePort,
					Default = string.Empty
				}
			}, new []
			{
				new BoolParameterSpecification<ParamId>
				{
					Identifier = ParamId.NoCert,
					Verbose = Constants.NewCommandOptions.VerboseNoCert
				}
			});
		}

		private struct Messages
		{
			public string outputMessage;
			public string loggerMessage;
		}

		public string Name => Constants.NewCommandArguments.OpcuaApp;

		public CommandResult Execute(IEnumerable<string> inputParams)
		{
			var outputMessages = new MessageLines();
			var messages = new Messages();

			var (error, stringParams, options) = _resolver.ResolveParams(inputParams);

			if (error != null)
				return new CommandResult(false, new MessageLines {{error, string.Empty}});

			var opcuaAppName = stringParams[ParamId.OpcuaAppName];
			var applicationType = stringParams[ParamId.ApplicationType];
			var url = stringParams[ParamId.Url];
			var port = stringParams[ParamId.Port];

			// validate opcuaapp name
			if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains) ||
			    _fileSystem.GetInvalidPathChars().Any(opcuaAppName.Contains))
			{
				AppioLogger.Warn(LoggingText.InvalidOpcuaappName);
				outputMessages.Add(string.Format(OutputText.NewOpcuaappCommandFailureInvalidProjectName, opcuaAppName),
					string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// validate opcuaapp type
			if (!ValidateApplicationType(ref messages, applicationType, url, port))
			{
				AppioLogger.Warn(messages.loggerMessage);
				outputMessages.Add(messages.outputMessage, string.Empty);
				return new CommandResult(false, outputMessages);
			}

			// combine project file paths
			var projectFilePath =
				_fileSystem.CombinePaths(opcuaAppName, $"{opcuaAppName}{Constants.FileExtension.Appioproject}");
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
				_fileSystem.CreateFile(mesonFilePath,
					_fileSystem.LoadTemplateFile(
						Resources.Resources.AppioOpcuaAppTemplateFileName_meson_ClientType_build));

				DeployTemplateOpcuaClientSourceFiles(sourceDirectory);
			}
			// deploy files for opcuaapp Server
			else if (applicationType == Constants.ApplicationType.Server)
			{
				opcuaapp = new OpcuaServerApp(opcuaAppName, url, port);
				_fileSystem.CreateFile(mesonFilePath,
					_fileSystem.LoadTemplateFile(
						Resources.Resources.AppioOpcuaAppTemplateFileName_meson_ServerType_build));

				CreateModelsDirectory(opcuaAppName);
				DeployTemplateOpcuaServerSourceFiles(sourceDirectory);
			}
			// deploy files for opcuaapp ClientServer
			else if (applicationType == Constants.ApplicationType.ClientServer)
			{
				opcuaapp = new OpcuaClientServerApp(opcuaAppName, url, port);
				_fileSystem.CreateFile(mesonFilePath,
					_fileSystem.LoadTemplateFile(Resources.Resources
						.AppioOpcuaAppTemplateFileName_meson_ClientServerType_build));

				CreateModelsDirectory(opcuaAppName);
				DeployTemplateOpcuaClientSourceFiles(sourceDirectory);
				DeployTemplateOpcuaServerSourceFiles(sourceDirectory);
			}

			if (!options[ParamId.NoCert])
			{
				if (applicationType == Constants.ApplicationType.ClientServer)
				{
					_certificateGenerator.Generate(opcuaAppName, Constants.FileName.ClientCryptoPrefix);
					_certificateGenerator.Generate(opcuaAppName, Constants.FileName.ServerCryptoPrefix);
				}
				else
				{
					_certificateGenerator.Generate(opcuaAppName);
				}
			}

			// create *.appioproj file
			var opcuaappAsJson = JsonConvert.SerializeObject(opcuaapp, Formatting.Indented);
			_fileSystem.CreateFile(projectFilePath, opcuaappAsJson);

			AppioLogger.Info(string.Format(LoggingText.NewOpcuaappCommandSuccess, opcuaAppName));
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
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.AppioOpcuaAppTemplateFileName_main_client_c));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_globalVariables_h), _fileSystem.LoadTemplateFile(Resources.Resources.AppioOpcuaAppTemplateFileName_globalVariables_client_h));
		}

		private void DeployTemplateOpcuaServerSourceFiles(string sourceDirectory)
		{
			var appSourceDirectory = _fileSystem.CombinePaths(sourceDirectory, Constants.DirectoryName.ServerApp);
			_fileSystem.CreateDirectory(appSourceDirectory);
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_main_c), _fileSystem.LoadTemplateFile(Resources.Resources.AppioOpcuaAppTemplateFileName_main_server_c));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_meson_build), _fileSystem.LoadTemplateFile(Resources.Resources.AppioOpcuaAppTemplateFileName_meson_server_build));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_loadInformationModels_c), _fileSystem.LoadTemplateFile(Resources.Resources.AppioOpcuaAppTemplateFileName_loadInformationModels_server_c));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_constants_h), _fileSystem.LoadTemplateFile(Resources.Resources.AppioOpcuaAppTemplateFileName_constants_server_h));
			_fileSystem.CreateFile(_fileSystem.CombinePaths(appSourceDirectory, Constants.FileName.SourceCode_mainCallbacks_c), _fileSystem.LoadTemplateFile(Resources.Resources.AppioOpcuaAppTemplateFileName_mainCallbacks_c));
		}

		private bool ValidateApplicationType(ref Messages messages, string applicationType, string url, string port)
		{
			// validate opcuaapp type
			if (applicationType != Constants.ApplicationType.Client &&
			    applicationType != Constants.ApplicationType.Server &&
			    applicationType != Constants.ApplicationType.ClientServer)
			{
				messages.loggerMessage = LoggingText.InvalidOpcuaappType;
				messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureUnknownProjectType, applicationType);
				return false;
			}
			
			if (applicationType == Constants.ApplicationType.Server || applicationType == Constants.ApplicationType.ClientServer)
			{
				// validate server url
				if (ValidateUrl(url))
				{
					messages.loggerMessage = LoggingText.InvalidServerUrl;
					messages.outputMessage = string.Format(OutputText.NewOpcuaappCommandFailureInvalidServerUrl, url);
					return false;
				}
				// validate server port
				if (ValidatePort(port))
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