using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appio.ObjectModel
{
    public class OpcuaappConverter<sourceType, targetType> : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<sourceType>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException(Constants.opcuaappConverterSerializationException);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
			var array = JArray.Load(reader);
			return array.Select(x => serializer.Deserialize<targetType>(new JTokenReader(x))).Cast<sourceType>().ToList();
		}
	}
}