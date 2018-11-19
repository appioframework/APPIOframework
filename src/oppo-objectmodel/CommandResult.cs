
namespace Oppo.ObjectModel
{
    public class CommandResult
    {
        public CommandResult(bool sucess, MessageLines outputMessages)
        {
            Sucsess = sucess;
            OutputMessages = outputMessages;
        }

        public bool Sucsess { get; private set; }
        public MessageLines OutputMessages { get; }
    }
}