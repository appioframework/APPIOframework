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

        public string Name { get; private set; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var projectName = inputParams.ElementAt(0);

            if (string.IsNullOrEmpty(projectName))
            {
                OppoLogger.Warn(LoggingText.EmptyOpcuaappName);
                var outputMsg = new List<KeyValuePair<string, string>>();
                outputMsg.Add(new KeyValuePair<string, string>(OutputText.OpcuaappPublishFailure, string.Empty));
                return new CommandResult(false, outputMsg);
            }

            var projectBuildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var applicationFileBuildLocation = _fileSystem.CombinePaths(projectBuildDirectory, Constants.ExecutableName.App);

            var projectPublishDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Publish);
            var applicationFilePublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.App);

            _fileSystem.CreateDirectory(projectPublishDirectory);
            _fileSystem.CopyFile(applicationFileBuildLocation, applicationFilePublishLocation);

            OppoLogger.Info(LoggingText.OpcuaappPublishedSuccess);
            var outputMessages = new List<KeyValuePair<string, string>>();
            outputMessages.Add(new KeyValuePair<string, string>(string.Format(OutputText.OpcuaappPublishSuccess, projectName), string.Empty));

            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}