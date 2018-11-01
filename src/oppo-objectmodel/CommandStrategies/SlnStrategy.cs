using System;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class SlnStrategy
        : ICommandStrategy
    {
        private readonly IFileSystem _fileSystemWrapper;
        private readonly ISlnCommandStrategyFactory _factory;

        public SlnStrategy(IFileSystem fileSystemWrapper, ISlnCommandStrategyFactory factory)
        {
            _fileSystemWrapper = fileSystemWrapper;    
            _factory = factory;
        }

        public string Execute(IEnumerable<string> inputsArgs)
        {
            var stretegy = _factory.GetStrategy(inputsArgs.First());

            _fileSystemWrapper.CreateFile("dummyName", "sln");
            return Constants.CommandResults.Success;
        }
    }
}