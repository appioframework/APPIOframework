using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.NewCommands
{
    public class NewOpcuaAppCommandStrategy : INewCommandStrategy
    {
        private readonly IFileSystem _fileSystem;

        public NewOpcuaAppCommandStrategy(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var nameFlag = inputParamsArray.ElementAtOrDefault(0);
            var opcuaAppName = inputParamsArray.ElementAtOrDefault(1);

            if (nameFlag != Constants.NewOpcuaAppCommandArguments.Name && nameFlag != Constants.NewOpcuaAppCommandArguments.VerboseName)
            {
                return Constants.CommandResults.Failure;
            }

            if (string.IsNullOrEmpty(opcuaAppName))
            {
                return Constants.CommandResults.Failure;
            }

            if (_fileSystem.GetInvalidFileNameChars().Any(opcuaAppName.Contains))
            {
                return Constants.CommandResults.Failure;
            }            

            if (_fileSystem.GetInvalidPathChars().Any(opcuaAppName.Contains))
            {
                return Constants.CommandResults.Failure;
            }

            _fileSystem.CreateDirectory(opcuaAppName);
            var filePath = _fileSystem.CombinePaths(opcuaAppName, $"{opcuaAppName}{Constants.FileExtension.OppoProject}");
            _fileSystem.CreateFile(filePath, _fileSystem.LoadTemplateFile(Resources.Resources.OppoOpcuaAppTemplateFileName));
            return Constants.CommandResults.Success;
        }
    }
}