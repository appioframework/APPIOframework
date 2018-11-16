using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanStrategy : ICommand<ObjectModel>
    {
        private readonly IFileSystem _fileSystem;

        public CleanStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.CommandName.Clean;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var nameFlag = inputParamsArray.ElementAtOrDefault(0);
            var projectName = inputParamsArray.ElementAtOrDefault(1);
            if (nameFlag != Constants.CleanCommandArguments.Name && nameFlag != Constants.CleanCommandArguments.VerboseName)
            {
                return new CommandResult(false, Resources.text.output.OutputText.OpcuaappCleanFailure);
            }

            if (string.IsNullOrEmpty(projectName))
            {
                return new CommandResult(false, Resources.text.output.OutputText.OpcuaappCleanFailure);
            }

            var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);

            _fileSystem.DeleteDirectory(buildDirectory);

            return new CommandResult(true, string.Format(Resources.text.output.OutputText.OpcuaappCleanSuccess, projectName));
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanCommand;
        }
    }
}
