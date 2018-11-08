using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildNameStrategy : ICommand<BuildStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public BuildNameStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public virtual string Name => Constants.BuildCommandArguments.Name;

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var projectName = inputParamsArray.ElementAtOrDefault(0);

            if (string.IsNullOrEmpty(projectName))
            {
                return Constants.CommandResults.Failure;
            }

            var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var mesonResult = _fileSystem.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild);
            if (!mesonResult)
            {
                return Constants.CommandResults.Failure;
            }
            var ninjaResult = _fileSystem.CallExecutable(Constants.ExecutableName.Ninja, buildDirectory, string.Empty);
            if (!ninjaResult)
            {
                return Constants.CommandResults.Failure;
            }

            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}
