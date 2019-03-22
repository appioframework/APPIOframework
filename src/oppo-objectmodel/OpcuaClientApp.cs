using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oppo.ObjectModel
{
    public class OpcuaClientApp : IOpcuaClientApp
    {
        public OpcuaClientApp()
        {
        }

        public OpcuaClientApp(string name)
        {
            Name = name;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = Constants.ApplicationType.Client;
		
		[JsonProperty("references")]
		[JsonConverter(typeof(OpcuaappConverter<IOpcuaServerApp, OpcuaServerApp>))]
		public List<IOpcuaServerApp> ServerReferences { get; set; } = new List<IOpcuaServerApp>();
    }
}