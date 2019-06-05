using Appio.ObjectModel;

namespace Appio.Terminal
{
    public interface IWriter
    {       
        void Write(MessageLines messagesToWrite);
    }
}