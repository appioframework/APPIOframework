using Oppo.ObjectModel;

namespace Oppo.Terminal
{
    public interface IWriter
    {       
        void Write(MessageLines messagesToWrite);
    }
}