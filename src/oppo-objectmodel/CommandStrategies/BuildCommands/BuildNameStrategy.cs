using System.Collections.Generic;
using System.Linq;
using Oppo.Resources.text.output;
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

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var projectName = inputParamsArray.ElementAtOrDefault(0);
            var outputMessages = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrEmpty(projectName))
            {
                OppoLogger.Warn( LoggingText.InvalidOpcuaappName);              
                outputMessages.Add(new KeyValuePair<string, string>(OutputText.OpcuaappBuildFailure, string.Empty));
                return new CommandResult(false, outputMessages);
            }

            var buildDirectory = _fileSystem.CombinePaths(projectName, Constants.DirectoryName.MesonBuild);
            var mesonResult = _fileSystem.CallExecutable(Constants.ExecutableName.Meson, projectName, Constants.DirectoryName.MesonBuild);
            if (!mesonResult)
            {
                OppoLogger.Warn(LoggingText.MesonExecutableFails);
                outputMessages.Add(new KeyValuePair<string, string>(OutputText.OpcuaappBuildFailure, string.Empty));
                return new CommandResult(false, outputMessages);
            }
            var ninjaResult = _fileSystem.CallExecutable(Constants.ExecutableName.Ninja, buildDirectory, string.Empty);
            if (!ninjaResult)
            {
                OppoLogger.Warn(LoggingText.NinjaExecutableFails);
                outputMessages.Add(new KeyValuePair<string, string>(OutputText.OpcuaappBuildFailure, string.Empty));
                return new CommandResult(false, outputMessages);
            }

            OppoLogger.Info(LoggingText.BuildSuccess);
            outputMessages.Add(new KeyValuePair<string, string>(string.Format(OutputText.OpcuaappBuildSuccess, projectName), string.Empty));
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return Resources.text.help.HelpTextValues.BuildNameArgumentCommandDescription;
        }
    }
}