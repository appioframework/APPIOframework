using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oppo.ObjectModel
{
    public class OpcuaClientServerApp : IOpcuaClientServerApp
    {
        public OpcuaClientServerApp()
        {
        }

        public OpcuaClientServerApp(string name, string url)
        {
            Name = name;
            Url = url;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; } = "ClientServer";
                
        [JsonIgnore]
        public List<IOpcuaServerApp> ServerReferences { get; set; } = new List<IOpcuaServerApp>();

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
    }
}