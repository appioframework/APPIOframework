using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oppo.ObjectModel
{
    public class OpcuaappReference : IOpcuaappReference
    {
        public OpcuaappReference()
        {
        }

        public OpcuaappReference(string name,  string path)
        {
            Name = name;
            Path = path;
        }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("path")]
        public string Path { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string Type { get; }
    }
}