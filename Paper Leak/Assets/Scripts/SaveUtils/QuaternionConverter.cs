using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
    public class QuaternionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Quaternion);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Quaternion quat = new();

            if (reader.TokenType != JsonToken.Null)
            {
                JObject jo = JObject.Load(reader);
                quat.x = jo["x"].Value<int>();
                quat.y = jo["y"].Value<int>();
                quat.z = jo["z"].Value<int>();
                quat.w = jo["w"].Value<int>();
            }

            return quat;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is not Quaternion quat)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("x");
            writer.WriteValue(quat.x);

            writer.WritePropertyName("y");
            writer.WriteValue(quat.y);

            writer.WritePropertyName("z");
            writer.WriteValue(quat.z);

            writer.WritePropertyName("w");
            writer.WriteValue(quat.w);

            writer.WriteEndObject();
        }
    }
}

