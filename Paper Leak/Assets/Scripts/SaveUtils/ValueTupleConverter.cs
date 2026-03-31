using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace Newtonsoft.Json.Converters
{
    public class ValueTupleConverter<T1, T2> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ValueTuple<T1, T2>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ValueTuple<T1, T2> tuple = new();

            if(reader.TokenType != JsonToken.Null)
            {
                JObject jo = JObject.Load(reader);
                tuple.Item1 = jo["Item1"].ToObject<T1>();
                tuple.Item2 = jo["Item2"].ToObject<T2>();
            }
            
            return tuple;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is not ValueTuple<T1, T2> tuple)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("Item1");
            //writer.WriteValue(tuple.Item1);
            serializer.Serialize(writer, tuple.Item1);

            writer.WritePropertyName("Item2");
            //writer.WriteValue(tuple.Item2);
            serializer.Serialize(writer, tuple.Item2);

            writer.WriteEndObject();
        }
    }
}
