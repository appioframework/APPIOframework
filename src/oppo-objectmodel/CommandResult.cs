using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public class CommandResult
    {
        public CommandResult(bool sucess, string message, Dictionary<string, string> outputText = null, IEnumerable<KeyValuePair<string, string>> messages = null)
        {
            Sucsess = sucess;
            Message = message;
            OutputText = outputText;
            Messages = messages;
        }

        public string Message { get; private set; }
        public bool Sucsess { get; private set; }

        public Dictionary<string, string> OutputText { get; private set; }
        public IEnumerable<KeyValuePair<string, string>> Messages { get; }
    }
}