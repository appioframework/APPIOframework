using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies.SlnCommands
{
    public class SlnNewCommandStrategy : ISlnCommandStrategy
    {
        private readonly IFileSystem fileSystemWrapper;

        public SlnNewCommandStrategy(IFileSystem fileSystemWrapper)
        {
            this.fileSystemWrapper = fileSystemWrapper;
        }

        public string Execute(IEnumerable<string> inputParams)
        {
            var firstInputParam = inputParams.FirstOrDefault();
            var seccondInputParam = inputParams.Skip(1).FirstOrDefault();
            
            if (firstInputParam == Constants.SlnNewCommandArguments.Name || firstInputParam == Constants.SlnNewCommandArguments.VerboseName)
            {            
                if (string.IsNullOrEmpty(seccondInputParam))
                {
                    return Constants.CommandResults.Failure;
                }

                if (fileSystemWrapper.GetInvalidFileNameChars().Any(seccondInputParam.Contains))
                {
                    return Constants.CommandResults.Failure;
                }

                fileSystemWrapper.CreateFile(Path.Combine(seccondInputParam, Constants.FileExtension.OppoSln), fileSystemWrapper.LoadTemplateFile(Oppo.Resources.Resources.OppoSlnTemplateFileName));
                return Constants.CommandResults.Success;
            }

            return Constants.CommandResults.Failure;
        }
    }
}