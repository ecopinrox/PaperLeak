using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Newtonsoft.Json.Converters
{
    public class DictionaryConverter<T> : JsonConverter<Dictionary<Vector2Int, T>>
    {
        static readonly string[] vector2IntSeparators = new[] {"(", ", ", ")"};

        public override Dictionary<Vector2Int, T> ReadJson(JsonReader reader, Type objectType, Dictionary<Vector2Int, T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (JsonToken.Null == reader.TokenType)
            {
                return null;
            }

            Dictionary<Vector2Int, T> dict = new();
            foreach(var pair in JObject.Load(reader))
            {
                IEnumerable<int> vectorFields = pair.Key.Split(vector2IntSeparators, StringSplitOptions.RemoveEmptyEntries).Select(it => Convert.ToInt32(it));
                Vector2Int key = new(vectorFields.First(), vectorFields.Last());
                T value = pair.Value.ToObject<T>(serializer);
                dict.Add(key, value);
            }

            return dict;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<Vector2Int, T> value, JsonSerializer serializer)
        {
            if (value is not Dictionary<Vector2Int, T>)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            foreach(KeyValuePair<Vector2Int, T> pair in value)
            {
                writer.WritePropertyName(pair.Key.ToString());
                serializer.Serialize(writer, pair.Value);
            }

            writer.WriteEndObject();
        }
    }
}
