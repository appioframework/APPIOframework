using OPPO.ObjectModel;

namespace OPPO.Terminal
{
    public class OppoTerminal
    {
        public OppoTerminal(IWriter writer)
        {
            writer.WriteLine(Constants.HelloString);
        }
    }
}