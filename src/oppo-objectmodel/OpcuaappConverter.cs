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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException(Constants.opcuaappConverterSerializationException);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
			var array = JArray.Load(reader);
			return array.Select(x => serializer.Deserialize<OpcuaappReference>(new JTokenReader(x))).Cast<IOpcuaapp>().ToList();
		}
	}
}