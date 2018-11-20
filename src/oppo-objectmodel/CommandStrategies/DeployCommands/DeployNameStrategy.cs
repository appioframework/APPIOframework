using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.DeployCommands
{
    public class DeployNameStrategy : ICommand<DeployStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public DeployNameStrategy(string deployNameCommandName, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = deployNameCommandName;
        }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var projectName = inputParams.ElementAtOrDefault(0);

            var outputMessages = new MessageLines();

            if (string.IsNullOrEmpty(projectName))
            {
                OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
                outputMessages.Add(OutputText.OpcuaappDeployFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var projectPublishDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Publish);
            var appClientPublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient);
            var appServerPublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer);

            var projectDeployDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Deploy);
            var appClientDeployLocation = _fileSystem.CombinePaths(projectDeployDirectory, Constants.ExecutableName.AppClient);
            var appServerDeployLocation = _fileSystem.CombinePaths(projectDeployDirectory, Constants.ExecutableName.AppServer);

            _fileSystem.CreateDirectory(projectDeployDirectory);
            _fileSystem.CopyFile(appClientPublishLocation, appClientDeployLocation);
            _fileSystem.CopyFile(appServerPublishLocation, appServerDeployLocation);

            var debianInstallerResult = _fileSystem.CallExecutable(Constants.ExecutableName.CreateDebianInstaller, projectDeployDirectory, string.Empty);
            if (!debianInstallerResult)
            {
                OppoLogger.Warn(LoggingText.CreateDebianInstallerFails);
                outputMessages.Add(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            OppoLogger.Info(LoggingText.OpcuaappDeploySuccess);
            
            outputMessages.Add(string.Format(OutputText.OpcuaappDeploySuccess, projectName), string.Empty);

            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.DeployNameArgumentCommandDescription;
        }
    }
}