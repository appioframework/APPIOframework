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
                     
            if (!_fileSystem.FileExists(appClientPublishLocation) && !_fileSystem.FileExists(appServerPublishLocation))
            {
                OppoLogger.Warn(LoggingText.MissingPublishedOpcuaAppFiles);
                outputMessages.Add(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }
                       
            // steps
            // create deploy dir
            _fileSystem.CreateDirectory(projectDeployDirectory);

            // create temp dir
            var tempDirectory = _fileSystem.CombinePaths(projectDeployDirectory, Constants.DirectoryName.Temp);
            _fileSystem.CreateDirectory(tempDirectory);

            // create needed installer structure            
            var zipSourceLocation = _fileSystem.CombinePaths(projectDeployDirectory, Constants.DirectoryName.Temp, Constants.DirectoryName.OpcuaappInstaller + Constants.FileExtension.ZipFile);          
            _fileSystem.ExtractFromZip(zipSourceLocation, tempDirectory, Resources.Resources.InstallerZipResourceName);

            // copy all needed files to temp dir installer source
			if(_fileSystem.FileExists(appClientPublishLocation))
			{
				var appClientDeployTempLocation = _fileSystem.CombinePaths(tempDirectory, Constants.DirectoryName.OpcuaappInstaller, Constants.DirectoryName.Usr, Constants.DirectoryName.Bin, Constants.ExecutableName.AppClient);
				_fileSystem.CopyFile(appClientPublishLocation, appClientDeployTempLocation);
			}
			if(_fileSystem.FileExists(appServerPublishLocation))
			{
				var appServerDeployTempLocation = _fileSystem.CombinePaths(tempDirectory, Constants.DirectoryName.OpcuaappInstaller, Constants.DirectoryName.Usr, Constants.DirectoryName.Bin, Constants.ExecutableName.AppServer);
				_fileSystem.CopyFile(appServerPublishLocation, appServerDeployTempLocation);
			}
          
            // create installer
            var debianInstallerResult = _fileSystem.CallExecutable(Constants.ExecutableName.CreateDebianInstaller, tempDirectory, Constants.ExecutableName.CreateDebianInstallerArguments);
            if (!debianInstallerResult)
            {
                OppoLogger.Warn(LoggingText.CreateDebianInstallerFails);
                outputMessages.Add(string.Format(OutputText.OpcuaappDeployWithNameFailure, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            // move installer to deploy dir
            var installerName = Constants.DirectoryName.OpcuaappInstaller + Constants.FileExtension.DebianInstaller;            
            var createdInstallerPath = _fileSystem.CombinePaths(tempDirectory, installerName);
            var installerTargetPath = _fileSystem.CombinePaths(projectDeployDirectory, installerName);
            _fileSystem.CopyFile(createdInstallerPath, installerTargetPath);

            // remove temp dir
            _fileSystem.DeleteDirectory(tempDirectory);

			// exit with success result
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