using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewSlnCommandStrategy : INewCommandStrategy
    {
        private readonly IFileSystem _fileSystem;

        public NewSlnCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var nameFlag = inputParamsArray.ElementAtOrDefault(0);
            var solutionName = inputParamsArray.ElementAtOrDefault(1);

            if (nameFlag != Constants.NewSlnCommandArguments.Name && nameFlag != Constants.NewSlnCommandArguments.VerboseName)
            {
                return Constants.CommandResults.Failure;
            }

            if (string.IsNullOrEmpty(solutionName))
            {
                return Constants.CommandResults.Failure;
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(solutionName.Contains))
            {
                return Constants.CommandResults.Failure;
            }

            _fileSystem.CreateFile($"{solutionName}{Constants.FileExtension.OppoSln}", _fileSystem.LoadTemplateFile(Resources.Resources.OppoSlnTemplateFileName));
            return Constants.CommandResults.Success;
        }
    }
}
