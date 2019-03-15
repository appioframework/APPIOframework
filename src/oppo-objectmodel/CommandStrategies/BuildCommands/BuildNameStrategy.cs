using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using Newtonsoft.Json;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
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
                OppoLogger.Warn(LoggingText.BuildProjectDoesNotExist);
                outputMessages.Add(string.Format(OutputText.OpcuaappBuildFailureProjectDoesNotExist, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

			SetServerHostnameAndPort(projectName);
			SetClientReferenceToServers(projectName);

			var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var mesonResult = _fileSystem.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild);
            if (!mesonResult)
            {
                OppoLogger.Warn(LoggingText.MesonExecutableFails);
                outputMessages.Add(OutputText.OpcuaappBuildFailure, string.Empty);

                return new CommandResult(false, outputMessages);
            }
            var ninjaResult = _fileSystem.CallExecutable(Constants.ExecutableName.Ninja, buildDirectory, string.Empty);
            if (!ninjaResult)
            {
                OppoLogger.Warn(LoggingText.NinjaExecutableFails);
                outputMessages.Add(OutputText.OpcuaappBuildFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            OppoLogger.Info(LoggingText.BuildSuccess);
            outputMessages.Add(string.Format(OutputText.OpcuaappBuildSuccess, projectName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

		private void SetServerHostnameAndPort(string projectName)
		{
			var oppoprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject);

			var oppoProjOpcuaapp = SlnUtility.DeserializeFile<OpcuaServerApp>(oppoprojFilePath, _fileSystem);

			if(oppoProjOpcuaapp != null && (oppoProjOpcuaapp.Type == Constants.ApplicationType.Server || oppoProjOpcuaapp.Type == Constants.ApplicationType.ClientServer))
			{
				var serverConstantsFilePath = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ServerApp, Constants.FileName.SourceCode_constants_h);

				var constantsFileContent = Constants.ServerConstants.ServerAppHostname + " = \"" + oppoProjOpcuaapp.Url + "\";\n" + Constants.ServerConstants.ServerAppPort + " = " + oppoProjOpcuaapp.Port + ";";
				_fileSystem.WriteFile(serverConstantsFilePath, new List<string> { constantsFileContent });
			}
		}

		private void SetClientReferenceToServers(string projectName)
		{
			var oppoprojFilePath = _fileSystem.CombinePaths(projectName, projectName + Constants.FileExtension.OppoProject);

			var oppoProjOpcuaapp = SlnUtility.DeserializeFile<OpcuaClientApp>(oppoprojFilePath, _fileSystem);

			if(oppoProjOpcuaapp != null && (oppoProjOpcuaapp.Type == Constants.ApplicationType.Client || oppoProjOpcuaapp.Type == Constants.ApplicationType.ClientServer))
			{
				var clientGlobalVariablesFilePath = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.SourceCode, Constants.DirectoryName.ClientApp, Constants.FileName.SourceCode_globalVariables_h);

				var globalVariablesFileContent = "#define numberOfReferences " + oppoProjOpcuaapp.ServerReferences.Count + "const char* SERVER_APP_URL[numberOfReferences] = {";
				foreach (var project in oppoProjOpcuaapp.ServerReferences)
				{
					globalVariablesFileContent += " \"opc.tcp://" + project.Url + ":" + project.Port + "/\",";
				}
				globalVariablesFileContent = globalVariablesFileContent.Remove(globalVariablesFileContent.Length - 1);
				globalVariablesFileContent += " };\nUA_Client* client[numberOfReferences];";

				_fileSystem.WriteFile(clientGlobalVariablesFilePath, new List<string> { globalVariablesFileContent });
			}
		}

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildNameArgumentCommandDescription;
        }
    }
}
 