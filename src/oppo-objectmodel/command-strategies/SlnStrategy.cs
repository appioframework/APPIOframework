using System;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class SlnStrategy
        : ICommandStrategy
    {
        private readonly IFileSystem _fileSystemWrapper;

        public SlnStrategy(IFileSystem fileSystemWrapper)
        {
            _fileSystemWrapper = fileSystemWrapper;    
        }

        public string Execute(IEnumerable<string> inputsArgs)
        {
            _fileSystemWrapper.CreateFile("dummyName", "sln");
            return Constants.CommandResults.Success;
        }
    }
}