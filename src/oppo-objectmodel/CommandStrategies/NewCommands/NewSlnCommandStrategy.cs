using Oppo.Resources.text.logging;
using Oppo.Resources.text.output;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewSlnCommandStrategy : ICommand<NewStrategy>
    {
        private readonly IFileSystem _fileSystem;

        public NewSlnCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Name => Constants.NewCommandName.Sln;

        public CommandResult Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var nameFlag = inputParamsArray.ElementAtOrDefault(0);
            var solutionName = inputParamsArray.ElementAtOrDefault(1);

            if (nameFlag != Constants.NewSlnCommandArguments.Name && nameFlag != Constants.NewSlnCommandArguments.VerboseName)
            {
                OppoLogger.Warn(LoggingText.UnknownNewSlnCommandParam);
                return new CommandResult(false, OutputText.NewSlnCommandFailureUnknownParam);
            }

            if (string.IsNullOrEmpty(solutionName))
            {
                OppoLogger.Warn(LoggingText.EmptySolutionName);
                return new CommandResult(false, OutputText.NewSlnCommandFailureUnknownParam);
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(solutionName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidSolutionName);
                return new CommandResult(false, string.Format(OutputText.NewSlnCommandFailure, solutionName));
            }

            var solutionFilePath = $"{solutionName}{Constants.FileExtension.OppoSln}";
            _fileSystem.CreateFile(solutionFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName));
            OppoLogger.Info(string.Format(LoggingText.NewSlnCommandSuccess, solutionFilePath));            
            return new CommandResult(true, string.Format(OutputText.NewSlnCommandSuccess, solutionName));
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}