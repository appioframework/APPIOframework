using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    public class OppoTerminal
    {
        public OppoTerminal(IWriter writer, string[] args)
        {
            writer.WriteLine(Constants.HelloString);
            
        }
    }
}