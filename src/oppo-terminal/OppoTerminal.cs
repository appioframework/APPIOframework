namespace OPPO.Terminal
{
    public class OppoTerminal
    {
        public OppoTerminal(IWriter writer)
        {
            writer.WriteLine("Hello from OPPO");
        }
    }
}