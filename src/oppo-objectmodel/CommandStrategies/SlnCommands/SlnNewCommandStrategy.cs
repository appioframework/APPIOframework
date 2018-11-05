using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnNewCommandStrategy : ISlnCommandStrategy
    {
        private readonly IFileSystem _fileSystemWrapper;

        public SlnNewCommandStrategy(IFileSystem fileSystemWrapper)
        {
            _fileSystemWrapper = fileSystemWrapper;
        }

        public string Execute(IEnumerable<string> inputParams)
        {
            var inputParamsArray = inputParams.ToArray();
            var firstInputParam = inputParamsArray.FirstOrDefault();
            var secondInputParam = inputParamsArray.Skip(1).FirstOrDefault();
            
            if (firstInputParam == Constants.SlnNewCommandArguments.Name || firstInputParam == Constants.SlnNewCommandArguments.VerboseName)
            {            
                if (string.IsNullOrEmpty(secondInputParam))
                {
                    return Constants.CommandResults.Failure;
                }

                if (_fileSystemWrapper.GetInvalidFileNameChars().Any(secondInputParam.Contains))
                {
                    return Constants.CommandResults.Failure;
                }

                _fileSystemWrapper.CreateFile($"{secondInputParam}{Constants.FileExtension.OppoSln}", _fileSystemWrapper.LoadTemplateFile(Oppo.Resources.Resources.OppoSlnTemplateFileName));
                return Constants.CommandResults.Success;
            }

            return Constants.CommandResults.Failure;
        }
    }
}