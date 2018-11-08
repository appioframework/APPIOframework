namespace Oppo.ObjectModel
{
    public interface ICommandFactory<TDependance>
    {
        ICommand<TDependance> GetCommand(string commandName);
    }
}
