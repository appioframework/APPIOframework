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

        public string Execute(IEnumerable<string> inputParams)
        {
            var projectName = inputParams.ElementAt(0);

            if (string.IsNullOrEmpty(projectName))
            {
                return Constants.CommandResults.Failure;
            }

            var projectBuildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var applicationFileBuildLocation = _fileSystem.CombinePaths(projectBuildDirectory, Constants.ExecutableName.App);

            var projectPublishDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.Publish);
            var applicationFilePublishLocation = _fileSystem.CombinePaths(projectPublishDirectory, Constants.ExecutableName.App);

            _fileSystem.CreateDirectory(projectPublishDirectory);
            _fileSystem.CopyFile(applicationFileBuildLocation, applicationFilePublishLocation);

            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}
