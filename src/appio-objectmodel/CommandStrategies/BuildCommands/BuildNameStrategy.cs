using System.Collections.Generic;
using System.Linq;
using Appio.Resources.text.output;
using Appio.Resources.text.logging;
using System.Text;
using System.IO;

namespace Appio.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildNameStrategy : ICommand<BuildStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public BuildNameStrategy(string buildCommandName, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = buildCommandName;
        }

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var projectName = inputParamsArray.ElementAtOrDefault(0);
            var outputMessages = new MessageLines();

            if (string.IsNullOrEmpty(projectName) || !_fileSystem.DirectoryExists(projectName))
            {
                AppioLogger.Warn(LoggingText.BuildProjectDoesNotExist);
                outputMessages.Add(string.Format(OutputText.OpcuaappBuildFailureProjectDoesNotExist, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

			SetServerHostnameAndPort(projectName);
			SetClientReferenceToServers(projectName);

			var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var mesonResult = _fileSystem.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild);
            if (!mesonResult)
            {
                AppioLogger.Warn(LoggingText.MesonExecutableFails);
                outputMessages.Add(OutputText.OpcuaappBuildFailure, string.Empty);

                return new CommandResult(false, outputMessages);
            }
            var ninjaResult = _fileSystem.CallExecutable(Constants.ExecutableName.Ninja, buildDirectory, string.Empty);
            if (!ninjaResult)
            {
                AppioLogger.Warn(LoggingText.NinjaExecutableFails);
                outputMessages.Add(OutputText.OpcuaappBuildFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            AppioLogger.Info(LoggingText.BuildSuccess);
            outputMessages.Add(string.Format(OutputText.OpcuaappBuildSuccess, projectName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

		private void SetServerHostnameAndPort(string projectName)
		{
			var appioprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject);

			var appioprojOpcuaapp = SlnUtility.DeserializeFile<OpcuaServerApp>(appioprojFilePath, _fileSystem);

			if(appioprojOpcuaapp != null && (appioprojOpcuaapp.Type == Constants.ApplicationType.Server || appioprojOpcuaapp.Type == Constants.ApplicationType.ClientServer))
			{
				var serverConstantsFilePath = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp, Constants.FileName.SourceCode_constants_h);

				var constantsFileContent = new List<string>();
				using (var constantsFileStream = _fileSystem.ReadFile(serverConstantsFilePath))
				{
					var reader = new StreamReader(constantsFileStream);
					while (!reader.EndOfStream)
					{
						constantsFileContent.Add(reader.ReadLine());
					}
					reader.Dispose();

					var hostnameLineIndex = constantsFileContent.FindIndex(x => x.Contains(Constants.ServerConstants.ServerAppHostname));
					if(hostnameLineIndex != -1)
					{
						constantsFileContent.RemoveAt(hostnameLineIndex);
					}

					var portLineIndex = constantsFileContent.FindIndex(x => x.Contains(Constants.ServerConstants.ServerAppPort));
					if (portLineIndex != -1)
					{
						constantsFileContent.RemoveAt(portLineIndex);
					}

					var hostName = new StringBuilder(Constants.ServerConstants.ServerAppHostname).Append(" = \"").Append(appioprojOpcuaapp.Url).Append("\";").ToString();
					constantsFileContent.Add(hostName);

					var portNumber = new StringBuilder(Constants.ServerConstants.ServerAppPort).Append(" = ").Append(appioprojOpcuaapp.Port).Append(";").ToString();
					constantsFileContent.Add(portNumber);
				}
				_fileSystem.WriteFile(serverConstantsFilePath, constantsFileContent);
			}
		}

		private void SetClientReferenceToServers(string projectName)
		{
			var appioprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.Appioproject);

			var appioprojOpcuaapp = SlnUtility.DeserializeFile<OpcuaClientApp>(appioprojFilePath, _fileSystem);

			if (appioprojOpcuaapp != null && (appioprojOpcuaapp.Type == Constants.ApplicationType.Client || appioprojOpcuaapp.Type == Constants.ApplicationType.ClientServer))
			{
				var clientGlobalVariablesFilePath = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ClientApp, Constants.FileName.SourceCode_globalVariables_h);

				var globalVariablesFileContent = new StringBuilder(string.Format(Constants.ClientGlobalVariables.FirstLines, appioprojOpcuaapp.ServerReferences.Count));
				foreach (var project in appioprojOpcuaapp.ServerReferences)
				{
					globalVariablesFileContent.Append(string.Format(Constants.ClientGlobalVariables.Hostname, project.Url, project.Port));
				}
				globalVariablesFileContent.Append(Constants.ClientGlobalVariables.LastLines);

				_fileSystem.WriteFile(clientGlobalVariablesFilePath, new List<string> { globalVariablesFileContent.ToString() } );
			}
		}

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildNameArgumentCommandDescription;
        }
    }
}
 