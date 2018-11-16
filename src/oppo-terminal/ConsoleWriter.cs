using System.Collections.Generic;
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

        public void WriteLines(List<string> messagesToWrite)
        {
            foreach (var msg in messagesToWrite)
            {
                System.Console.WriteLine(msg);
            }
        }

        public void WriteLines(Dictionary<string, string> messagesToWrite)
        {
            int longestKey = GetLongestKey(messagesToWrite);

            foreach (var key in messagesToWrite.Keys)
            {
                System.Console.WriteLine(string.Format("{0}{1}", key.PadRight(longestKey + 3, ' '), messagesToWrite[key]));
            }
        }

        private static int GetLongestKey(Dictionary<string, string> messagesToWrite)
        {
            var longestKey = 0;
            foreach (var key in messagesToWrite.Keys)
            {
                if (key.Length > longestKey)
                {
                    longestKey = key.Length;
                }
            }

            return longestKey;
        }
    }
}