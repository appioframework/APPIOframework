using Oppo.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Oppo.Terminal
{
    [ExcludeFromCodeCoverage]
    public class ConsoleWriter : IWriter
    {     
        public void Write(MessageLines messagesToWrite)
        {
            var longestKey = GetLongestKey(messagesToWrite);
            foreach (var message in messagesToWrite)
            {
                System.Console.WriteLine(string.Format("{0}{1}", message.Key.PadRight(longestKey + 3, ' '), message.Value));
            }
        }

        private static int GetLongestKey(MessageLines messagesToWrite)
        {
            var longestKey = 0;
            foreach (var message in messagesToWrite)
            {
                if (message.Key.Length > longestKey)
                {
                    longestKey = message.Key.Length;
                }
            }

            return longestKey;
        }
    }
}