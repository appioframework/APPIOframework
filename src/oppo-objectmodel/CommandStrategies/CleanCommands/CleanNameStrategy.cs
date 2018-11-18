using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.CleanCommands
{
    public class CleanNameStrategy : ICommand<CleanStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public CleanNameStrategy(string name, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            Name = name;
        }

        public string Name { get; }

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var projectName = inputParamsArray.ElementAtOrDefault(0);

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
            return Resources.text.help.HelpTextValues.CleanNameArgumentCommandDescription;
        }
    }
}
