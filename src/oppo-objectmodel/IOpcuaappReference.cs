namespace Oppo.ObjectModel
{
    public interface IOpcuaappReference : IOpcuaapp
    {
        string Path { get; set; }
    }
}