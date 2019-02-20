using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oppo.ObjectModel
{
    public class Opcuaapp
    {
        public Opcuaapp()
        {
        }

        public Opcuaapp(string name, OpcuaappType type, string url)
        {
            Name = name;
            Type = type;
            Url = url;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OpcuaappType Type { get; set; } = OpcuaappType.ClientServer;

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}