using System.Collections;
using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public class MessageLines : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> _messages = new List<KeyValuePair<string, string>>();

        public MessageLines()
        {
        }

        public MessageLines(MessageLines messages)
        {
            foreach (var msg in messages)
            {
                Add(msg.Key, msg.Value);
            }
        }

        public void Add(string messageHeader, string message)
        {
            _messages.Add(new KeyValuePair<string, string>(messageHeader, message));
        }

        public void Add(MessageLines lines)
        {
            foreach (var line in lines)
            {
                Add(line.Key, line.Value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}