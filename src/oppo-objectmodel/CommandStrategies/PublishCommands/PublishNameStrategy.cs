using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.PublishCommands
{
    public class PublishNameStrategy : ICommand<PublishStrategy>
/*IPublishStrategy*/
    {
        private readonly IFileSystem _fileSystem;

        public PublishNameStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public virtual string Name => Constants.PublishCommandArguments.Name;
        //public string VerboseName => Constants.PublishCommandArguments.VerboseName;

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
            throw new System.NotSupportedException();
        }
    }
}
