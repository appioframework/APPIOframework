using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    public class OppoTerminal
    {
        public OppoTerminal(IWriter writer)
        {
            writer.WriteLine(Constants.HelloString);
        }
    }
}