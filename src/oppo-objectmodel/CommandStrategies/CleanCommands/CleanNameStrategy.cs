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
            var outputMessages = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrEmpty(projectName))
            {
                OppoLogger.Info(Resources.text.logging.LoggingText.CleanFailure);
                outputMessages.Add(new KeyValuePair<string, string>(Resources.text.output.OutputText.OpcuaappCleanFailure, string.Empty));
                return new CommandResult(false, outputMessages);
            }

            var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            _fileSystem.DeleteDirectory(buildDirectory);
            OppoLogger.Info(Resources.text.logging.LoggingText.CleanSuccess);                        
            outputMessages.Add(new KeyValuePair<string, string>(string.Format(Resources.text.output.OutputText.OpcuaappCleanSuccess, projectName), string.Empty));
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.CleanNameArgumentCommandDescription;
        }
    }
}