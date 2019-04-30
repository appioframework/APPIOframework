
namespace Oppo.ObjectModel
{
    public class CommandResult
    {
        public CommandResult(bool sucess, MessageLines outputMessages)
        {
            Success = sucess;
            OutputMessages = outputMessages;
        }

        public bool Success { get; private set; }
        public MessageLines OutputMessages { get; }
    }
}