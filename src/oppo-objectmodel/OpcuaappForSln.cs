using Newtonsoft.Json;

namespace Oppo.ObjectModel
{
	public class OpcuaappForSln : Opcuaapp
	{
		public OpcuaappForSln()
		{
		}

		public OpcuaappForSln(Opcuaapp opcuaapp, string path)
		{
			Name = opcuaapp.Name;
			Path = path;
			Type = opcuaapp.Type;
			Url = opcuaapp.Url;
		}

		[JsonProperty("path")]
		public string Path { get; set; } = string.Empty;
	}
}