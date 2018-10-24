namespace OPPO.Terminal
{
    public class OppoTerminal
    {
        public OppoTerminal(IWriter writer)
        {
            if (writer != null)
            writer.WriteLine("Hello from OPPO");
        }

        public void Run()
        {
        }
    }
}