using System;
using System.Collections.Generic;

namespace Oppo.ObjectModel.CommandStrategies
{
    public class HelloStrategy : ICommandStrategy
    {
        private readonly IWriter _writer;

        public HelloStrategy(IWriter writer)
        {
            _writer = writer;
        }

        public string Execute(IEnumerable<string> inputsParams)
        {
            _writer.WriteLine(Constants.HelloString);
            return Constants.CommandResults.Success;
        }

        public string GetHelpText()
        {
            return string.Empty;
        }
    }
}
