using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oppo.ObjectModel
{
    public class Opcuaapp
    {
        public Opcuaapp()
        {
        }

        public Opcuaapp(string name, string path, OpcuaappType type, string url)
        {
            Name = name;
            Path = path;
            Type = type;
            Url = url;
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