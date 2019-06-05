using Newtonsoft.Json;
using System.Collections.Generic;

namespace Appio.ObjectModel
{
    public class Solution : ISolution
    {
        [JsonProperty("projects")]
        [JsonConverter(typeof(OpcuaappConverter<IOpcuaapp, OpcuaappReference>))]
        public List<IOpcuaapp> Projects { get; private set; } = new List<IOpcuaapp>();
    }
}