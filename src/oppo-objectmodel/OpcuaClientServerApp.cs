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

        public OpcuaClientServerApp(string name, string url, string port)
        {
            Name = name;
            Url = url;
			Port = port;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set;  } = Constants.ApplicationType.ClientServer;
                
        [JsonIgnore]
        public List<IOpcuaServerApp> ServerReferences { get; set; } = new List<IOpcuaServerApp>();

        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

		[JsonProperty("port")]
		public string Port { get; set; } = string.Empty;
    }
}