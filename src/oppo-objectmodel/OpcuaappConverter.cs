using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oppo.ObjectModel
{
    public class OpcuaappConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<IOpcuaapp>);
        }
        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue,
            JsonSerializer serializer)
        {
			var jA = JArray.Load(reader);
			return jA.Select(jl => serializer.Deserialize<OpcuaappReference>(new JTokenReader(jl))).Cast<IOpcuaapp>().ToList();

		}
	}
}