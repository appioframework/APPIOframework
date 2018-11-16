using System.Collections.Generic;

namespace Oppo.Terminal
{
    public interface IWriter
    {
        void WriteLine(string messageToWrite);
        void WriteLines(List<string> messagesToWrite);
        void WriteLines(Dictionary<string, string> messagesToWrite);
        void Write(IEnumerable<KeyValuePair<string, string>> messageToWrite);
    }
}