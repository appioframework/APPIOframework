using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public class CommandResult
    {
        public CommandResult(bool sucess, IEnumerable<KeyValuePair<string, string>> outputMessages = null)
        {
            Sucsess = sucess;
            OutputMessages = outputMessages;
        }

        public bool Sucsess { get; private set; }
        public IEnumerable<KeyValuePair<string, string>> OutputMessages { get; }
    }
}