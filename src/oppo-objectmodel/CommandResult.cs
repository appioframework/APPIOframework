namespace Oppo.ObjectModel
{
    public class CommandResult
    {
        public CommandResult(bool sucess, string message)
        {
            Sucsess = sucess;
            Message = message;
        }

        public string Message { get; private set; }
        public bool Sucsess { get; private set; }
    }
}