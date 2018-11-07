using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildStrategy : ICommandStrategy
    {
        private readonly IFileSystem _fileSystem;

        public BuildStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Execute(IEnumerable<string> inputsParams)
        {
            var inputParamsArray = inputsParams.ToArray();
            var nameFlag = inputParamsArray.ElementAtOrDefault(0);
            var projectName = inputParamsArray.ElementAtOrDefault(1);

            if (nameFlag != Constants.BuildCommandArguments.Name && nameFlag != Constants.BuildCommandArguments.VerboseName)
            {
                return Constants.CommandResults.Failure;
            }

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
            return Resources.text.help.HelpText.BuildCommand;
        }
    }
}
