using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using UnityEditor;

public class JsonSaver
{
    public static void Save(MasterSave save, string filename)
    {
        string json = JsonConvert.SerializeObject(save, Formatting.Indented, GetSerializerSettings());

        using StreamWriter writer = new(BuildPath(filename));
        writer.Write(json);
    }

    public static void Load(ref MasterSave save, string filename)
    {
        string path = BuildPath(filename);
        try
        {
            string json;
            using (StreamReader reader = new(path))
            {
                json = reader.ReadToEnd();
            }

            save = JsonConvert.DeserializeObject<MasterSave>(json, GetSerializerSettings());
        }
        catch(FileNotFoundException)
        {
            throw new FileNotFoundException($"Save file \"{path}\" not found.");
        }
    }

    public static void DeleteSaveFile(string filename)
    {
        try
        {
            File.Delete(BuildPath(filename));
        }
        catch (Exception e) 
        { 
            throw e; 
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
        settings.Converters.Add( new MasterSaveConverter()                                      );
        return settings;
    }

    static string BuildPath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename);
    }
}
