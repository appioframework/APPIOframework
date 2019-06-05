namespace Appio.ObjectModel
{
    public interface IOpcuaappReference : IOpcuaapp
    {
        string Path { get; set; }
    }
}