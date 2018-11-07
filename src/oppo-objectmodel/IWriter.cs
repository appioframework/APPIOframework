using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface IWriter
    {
        void WriteLine(string messageToWrite);
        void WriteLines(List<string> helpOutput);
    }
}