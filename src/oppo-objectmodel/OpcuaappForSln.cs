using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oppo.ObjectModel
{
	public class OpcuaappForSln
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

		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("path")]
		public string Path { get; set; } = string.Empty;

		[JsonProperty("type")]
		[JsonConverter(typeof(StringEnumConverter))]
		public OpcuaappType Type { get; set; } = OpcuaappType.ClientServer;

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}