using System.Diagnostics.CodeAnalysis;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    public class ConsoleWriter : IWriter
    {
        public void WriteLine(string messageToWrite)
        {
            System.Console.WriteLine(messageToWrite);
        }
    }
}