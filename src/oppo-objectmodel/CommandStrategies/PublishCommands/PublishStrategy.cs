using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishStrategy : ICommandStrategy
    {
        private readonly IFileSystem _fileSystem;

        public PublishStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.CommandName.Publish;

        public string Execute(IEnumerable<string> inputsParams)
        {
            var inputParamsArray = inputsParams.ToArray();
            var projectName = inputParamsArray.ElementAt(1);

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
            return Resources.text.help.HelpTextValues.PublishCommand;
        }
    }
}
