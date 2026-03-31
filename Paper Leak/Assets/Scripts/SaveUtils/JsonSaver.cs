using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

public class JsonSaver
{
    public static void Save(SaveState save, string filename)
    {
        string json = JsonConvert.SerializeObject(save, Formatting.Indented, GetSerializerSettings());

        using StreamWriter writer = new(BuildPath(filename));
        writer.Write(json);
    }

    public static void Load(SaveState save, string filename)
    {
        try
        {
            string json;
            using (StreamReader reader = new(BuildPath(filename)))
            {
                json = reader.ReadToEnd();
            }

            JsonConvert.PopulateObject(json, save, GetSerializerSettings());
        }
        catch(FileNotFoundException)
        {
            //do nothing
        }
    }

    static JsonSerializerSettings GetSerializerSettings()
    {       
        JsonSerializerSettings settings = new();
        settings.Converters.Add( new Vector2IntConverter()                                      );
        settings.Converters.Add( new DictionaryConverter<int>()                                 );
        settings.Converters.Add( new QuaternionConverter()                                      );
        settings.Converters.Add( new ValueTupleConverter<Vector2Int, Quaternion>()              );
        settings.Converters.Add( new DictionaryConverter<ValueTuple<Vector2Int, Quaternion>>()  );
        return settings;
    }

    static string BuildPath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename);
    }
}
