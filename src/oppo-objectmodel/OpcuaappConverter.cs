using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
            var jsonObject = JObject.Load(reader);
            var profession = default(IOpcuaapp);
            switch (jsonObject["Type"].Value<string>())
            {
                case "Server":
                    profession = new OpcuaServerApp();
                    break;
                case "Client":
                    profession = new OpcuaClientApp();
                    break;
                case "ClientServer":
                    profession = new OpcuaClientServerApp();
                    break;
                case null:
                    profession = new OpcuaappReference();
                    break;
            }
            serializer.Populate(jsonObject.CreateReader(), profession);
            return profession;
        }
    }
}