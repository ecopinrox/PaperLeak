using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
    public class MasterSaveConverter : JsonConverter<MasterSave>
    {
        public override MasterSave ReadJson(JsonReader reader, Type objectType, MasterSave existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            MasterSave save = hasExistingValue ? existingValue : ScriptableObject.CreateInstance<MasterSave>();

            if(reader.TokenType == JsonToken.Null)
            {
                return save;
            }

            JObject jo = JObject.Load(reader);
            save.currentLevelIndex = jo["currentLevelIndex"].Value<int>();
            save.difficulty = jo["difficulty"].Value<int>();
            save.visited = jo["visited"].ToObject<HashSet<int>>(serializer);

            JArray saveStateArray = (JArray)jo["levelStates"];
            save.levelStates ??= new SaveState[saveStateArray.Count];

            for(int i = 0; i < save.levelStates.Length; i++)
            {
                if (i >= saveStateArray.Count)
                {
                    //Debug.Log($"{i} > number of save states in json ({saveStateArray.Count})");
                    save.levelStates[i] = ScriptableObject.CreateInstance<SaveState>();
                    continue;
                }

                JToken saveStateToken = saveStateArray[i];

                if (save.levelStates[i] == null)
                {
                    save.levelStates[i] = ScriptableObject.CreateInstance<SaveState>();
                }

                using JsonReader saveStateReader = saveStateToken.CreateReader();
                serializer.Populate(saveStateReader, save.levelStates[i]);
            }

            return save;
        }

        public override void WriteJson(JsonWriter writer, MasterSave value, JsonSerializer serializer)
        {
            if(value == null)
            {
                writer.WriteNull(); 
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("currentLevelIndex");
            writer.WriteValue(value.currentLevelIndex);

            writer.WritePropertyName("difficulty");
            writer.WriteValue(value.difficulty);

            writer.WritePropertyName("visited");
            serializer.Serialize(writer, value.visited);

            writer.WritePropertyName("levelStates");
            serializer.Serialize(writer, value.levelStates);

            writer.WriteEndObject();
        }
    }
}

