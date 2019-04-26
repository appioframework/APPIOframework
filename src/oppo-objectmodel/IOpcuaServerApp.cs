using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public interface IOpcuaServerApp : IOpcuaapp
    {
        string Url { get; set; }
		string Port { get; set; }
		List<IModelData> Models { get; set; }
    }
}