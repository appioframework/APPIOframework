using Oppo.Resources.text.output;
using Oppo.Resources.text.logging;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
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
            var projectName = inputParams.ElementAt(0);

            var outputMessages = new MessageLines();

            if (string.IsNullOrEmpty(projectName))
            {
                OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
                outputMessages.Add(OutputText.OpcuaappPublishFailure, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var projectBuildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var appClientBuildLocation = _fileSystem.CombinePaths(projectBuildDirectory, Constants.ExecutableName.AppClient);
            var appServerBuildLocation = _fileSystem.CombinePaths(projectBuildDirectory, Constants.ExecutableName.AppServer);

            var projectPublishDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Publish);
            var appClientPublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppClient);
            var appServerPublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.AppServer);

            _fileSystem.CreateDirectory(projectPublishDirectory);
            _fileSystem.CopyFile(appClientBuildLocation, appClientPublishLocation);
            _fileSystem.CopyFile(appServerBuildLocation, appServerPublishLocation);

            OppoLogger.Info(LoggingText.OpcuaappPublishedSuccess);
            
            outputMessages.Add(string.Format(OutputText.OpcuaappPublishSuccess, projectName), string.Empty);

            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}