using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface IOpcuaClientApp : IOpcuaapp
    {
        List<IOpcuaServerApp> ServerReferences { get; }
    }


}