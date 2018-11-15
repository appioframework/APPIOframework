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

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var nameFlag = inputParamsArray.ElementAtOrDefault(0);
            var solutionName = inputParamsArray.ElementAtOrDefault(1);

            if (nameFlag != Constants.NewSlnCommandArguments.Name && nameFlag != Constants.NewSlnCommandArguments.VerboseName)
            {
                OppoLogger.Warn(Resources.text.logging.LoggingText.UnknownNewSlnCommandParam);
                return Constants.CommandResults.Failure;
            }

            if (string.IsNullOrEmpty(solutionName))
            {
                OppoLogger.Warn(Resources.text.logging.LoggingText.EmptySolutionName);
                return Constants.CommandResults.Failure;
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(solutionName.Contains))
            {
                OppoLogger.Warn(Resources.text.logging.LoggingText.InvalidSolutionName);
                return Constants.CommandResults.Failure;
            }

            var solutionFilePath = $"{solutionName}{Constants.FileExtension.OppoSln}";
            _fileSystem.CreateFile(solutionFilePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName));
            OppoLogger.Info(string.Format(Resources.text.logging.LoggingText.NewSlnCommandSuccess, solutionFilePath));            
            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}