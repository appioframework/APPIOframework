using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    public class ConsoleWriter : IWriter
    {
        public void WriteLine(string messageToWrite)
        {
            System.Console.WriteLine(messageToWrite);
        }

        public void WriteLines(List<string> messagesToWrite)
        {
            foreach (var msg in messagesToWrite)
            {
                System.Console.WriteLine(msg);
            }
        }
    }
}