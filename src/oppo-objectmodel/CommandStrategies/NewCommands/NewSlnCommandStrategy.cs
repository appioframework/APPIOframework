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
            var outputMessages = new MessageLines();

            if (nameFlag != Constants.NewSlnCommandArguments.Name && nameFlag != Constants.NewSlnCommandArguments.VerboseName)
            {
                OppoLogger.Warn(LoggingText.UnknownNewSlnCommandParam);
                outputMessages.Add(OutputText.NewSlnCommandFailureUnknownParam, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (string.IsNullOrEmpty(solutionName))
            {
                OppoLogger.Warn(LoggingText.EmptySolutionName);
                outputMessages.Add(OutputText.NewSlnCommandFailureUnknownParam, string.Empty);
                return new CommandResult(false, outputMessages);
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(solutionName.Contains))
            {
                OppoLogger.Warn(LoggingText.InvalidSolutionName);
                outputMessages.Add(string.Format(OutputText.NewSlnCommandFailure, solutionName), string.Empty);
                return new CommandResult(false, outputMessages);
            }

            var solutionFilePath = $"{solutionName}{Constants.FileExtension.OppoSln}";
            _fileSystem.CreateFile(solutionFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName));
            OppoLogger.Info(string.Format(LoggingText.NewSlnCommandSuccess, solutionFilePath));
            outputMessages.Add(string.Format(OutputText.NewSlnCommandSuccess, solutionName), string.Empty);
            return new CommandResult(true, outputMessages);
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}