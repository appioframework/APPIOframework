using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oppo.ObjectModel
{
    public class OpcuaServerApp : IOpcuaServerApp
    {
        public OpcuaServerApp()
        {
        }

        public OpcuaServerApp(string name, string url)
        {
            Name = name;
            Url = url;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; } = Constants.ApplicationType.Server;

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
        
    }
}