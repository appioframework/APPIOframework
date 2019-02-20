using Newtonsoft.Json;
using System.Collections.Generic;

namespace Oppo.ObjectModel
{
    public class Solution : ISolution
    {
        [JsonProperty("projects")]
        public List<OpcuaappForSln> Projects { get; private set; } = new List<OpcuaappForSln>();
    }
}