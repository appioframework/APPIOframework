using Appio.Resources.text.output;
using Appio.Resources.text.logging;
using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishNameStrategy : ICommand<PublishStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public PublishNameStrategy(string publishNameCommandName, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = publishNameCommandName;
        }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var projectName = inputParams.ElementAtOrDefault(0);

            var outputMessages = new MessageLines();

			// validate project name
            if (string.IsNullOrEmpty(projectName))
            {
                AppioLogger.Warn(LoggingText.EmptyOpcuaappName);
                outputMessages.Add(OutputText.OpcuaappPublishFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

			// build string with publish command source location
            var projectBuildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var appClientBuildLocation = _fileSystem.CombinePaths(projectBuildDirectory, Constants.ExecutableName.AppClient);
            var appServerBuildLocation = _fileSystem.CombinePaths(projectBuildDirectory, Constants.ExecutableName.AppServer);

			// build strings with publish command target location
            var projectPublishDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Publish);
            var appClientPublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient);
            var appServerPublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer);

			// check if any of client and server executables exist
            if ((string.IsNullOrEmpty(appClientBuildLocation) || !_fileSystem.FileExists(appClientBuildLocation)) &&
                (string.IsNullOrEmpty(appServerBuildLocation) || !_fileSystem.FileExists(appServerBuildLocation)))
            {
                AppioLogger.Warn(LoggingText.MissingBuiltOpcuaAppFiles);
                outputMessages.Add(string.Format(OutputText.OpcuaappPublishFailureMissingExecutables, projectName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

			// create publish directory
			_fileSystem.CreateDirectory(projectPublishDirectory);

			// publish client executable
			if (!string.IsNullOrEmpty(appClientBuildLocation) && _fileSystem.FileExists(appClientBuildLocation))
			{
				_fileSystem.CopyFile(appClientBuildLocation, appClientPublishLocation);
			}
			// publish server executable
			if (!string.IsNullOrEmpty(appServerBuildLocation) && _fileSystem.FileExists(appServerBuildLocation))
			{
				_fileSystem.CopyFile(appServerBuildLocation, appServerPublishLocation);
			}

			// return with success
            AppioLogger.Info(LoggingText.OpcuaappPublishedSuccess);
            outputMessages.Add(string.Format(OutputText.OpcuaappPublishSuccess, projectName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.PublishNameArgumentCommandDescription;
        }
    }
}