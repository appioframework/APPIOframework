using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.logging;

namespace Oppo.ObjectModel.CommandStrategies.BuildCommands
{
    public class BuildNameStrategy : ICommand<BuildStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public BuildNameStrategy(string buildCommandName, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = buildCommandName;
        }

        public string Name { get; private set; }

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var projectName = inputParamsArray.ElementAtOrDefault(0);

            if (string.IsNullOrEmpty(projectName))
            {
                OppoLogger.Warn( LoggingText.InvalidOpcuaappName);
                return Constants.CommandResults.Failure;
            }

            var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var mesonResult = _fileSystem.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild);
            if (!mesonResult)
            {
                OppoLogger.Warn(LoggingText.MesonExecutableFails);
                return Constants.CommandResults.Failure;
            }
            var ninjaResult = _fileSystem.CallExecutable(Constants.ExecutableName.Ninja, buildDirectory, string.Empty);
            if (!ninjaResult)
            {
                OppoLogger.Warn(LoggingText.NinjaExecutableFails);
                return Constants.CommandResults.Failure;
            }

            OppoLogger.Info(LoggingText.BuildSuccess);
            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildNameArgumentCommandDescription;
        }
    }
}