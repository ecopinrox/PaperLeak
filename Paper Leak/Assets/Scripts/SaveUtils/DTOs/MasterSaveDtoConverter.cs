using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
    public class MasterSaveDtoConverter : JsonConverter<MasterSaveDto>
    {
        public override MasterSaveDto ReadJson(JsonReader reader, Type objectType, MasterSaveDto existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if(reader.TokenType == JsonToken.Null)
            {
                return new();
            }

            MasterSaveDto dto = new();

            JObject jo = JObject.Load(reader);
            dto.currentLevelIndex = jo["currentLevelIndex"]?.Value<int>();
            dto.difficulty = jo["difficulty"]?.Value<int>();
            dto.visited = jo["visited"]?.ToObject<HashSet<int>>(serializer);

            JArray saveStateArray = (JArray)jo["levelStates"];
            if(saveStateArray != null)
            {
                dto.levelStates = new SaveStateDto[saveStateArray.Count];

                for(int i = 0; i < dto.levelStates.Length; i++)
                {
                    JToken saveStateToken = saveStateArray[i];

                    using JsonReader saveStateReader = saveStateToken.CreateReader();
                    dto.levelStates[i] = serializer.Deserialize<SaveStateDto>(saveStateReader);
                }
            }

            return dto;
        }

        public override void WriteJson(JsonWriter writer, MasterSaveDto value, JsonSerializer serializer)
        {
            if(value == null)
            {
                writer.WriteNull(); 
                return;
            }

            writer.WriteStartObject();

            if(value.currentLevelIndex != null)
            {
                writer.WritePropertyName("currentLevelIndex");
                writer.WriteValue(value.currentLevelIndex);
            }

            if(value.difficulty != null)
            {
                writer.WritePropertyName("difficulty");
                writer.WriteValue(value.difficulty);
            }

            if(value.visited != null)
            {
                writer.WritePropertyName("visited");
                serializer.Serialize(writer, value.visited);
            }

            if (value.levelStates != null)
            {
                writer.WritePropertyName("levelStates");
                serializer.Serialize(writer, value.levelStates);
            }

            writer.WriteEndObject();
        }
    }
}
