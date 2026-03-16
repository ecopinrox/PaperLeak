using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
    public class Vector2IntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2Int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Vector2Int vector = new();

            if (reader.TokenType != JsonToken.Null)
            {
                JObject jo = JObject.Load(reader);
                vector.x = jo["x"].Value<int>();
                vector.y = jo["y"].Value<int>();
            }

            return vector;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is not Vector2Int vector)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("x");
            writer.WriteValue(vector.x);

            writer.WritePropertyName("y");
            writer.WriteValue(vector.y);

            writer.WriteEndObject();
        }
    }
}
