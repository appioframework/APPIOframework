namespace OPPO.Terminal
{
    public class ConsoleWriter : IWriter
    {
        public void WriteLine(string messageToWrite)
        {
            System.Console.WriteLine(messageToWrite);
        }
    }
}