using System.Collections.Generic;

namespace Appio.ObjectModel
{
    public interface IOpcuaClientApp : IOpcuaapp
    {
        List<IOpcuaServerApp> ServerReferences { get; }
    }
}