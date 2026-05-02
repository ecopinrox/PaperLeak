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
        //read old data from save
        string oldJson;
        MasterSaveDto dto = new();
        try
        {
            oldJson = GetJson(filename);
            dto = JsonConvert.DeserializeObject<MasterSaveDto>(oldJson, GetSerializerSettings());
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"Save file \"{BuildPath(filename)}\" not found.");
        }

        //overwrite current level's data along with commmon MasterSave fields
        WriteMasterToDto(save, dto);

        //write new data to save
        string newJson = JsonConvert.SerializeObject(dto, Formatting.Indented, GetSerializerSettings());
        using StreamWriter writer = new(BuildPath(filename));
        writer.Write(newJson);
    }

    public static void Load(MasterSave save, string filename)
    {
        try
        {
            string json = GetJson(filename);

            MasterSaveDto dto = JsonConvert.DeserializeObject<MasterSaveDto>(json, GetSerializerSettings());
            ReadMasterFromDto(save, dto);
        }
        catch(FileNotFoundException)
        {
            throw new FileNotFoundException($"Save file \"{BuildPath(filename)}\" not found.");
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

    static string GetJson(string filename)
    {
        string json;

        string path = BuildPath(filename);
        using (StreamReader reader = new(path))
        {
            json = reader.ReadToEnd();
        }

        return json;
    }

    static void WriteMasterToDto(MasterSave save, MasterSaveDto dto)
    {
        dto.currentLevelIndex = save.currentLevelIndex;
        dto.difficulty = save.difficulty;
        dto.visited = save.visited;

        dto.levelStates ??= new SaveStateDto[save.levelStates.Length];
        WriteLevelToDto(save.levelStates[save.currentLevelIndex], dto.levelStates[save.currentLevelIndex]);
    }

    static void WriteLevelToDto(SaveState save, SaveStateDto dto)
    {
        dto.timeElapsed = save.timeElapsed;

        dto.playerPos = save.playerPos;

        dto.heldCollectibles = save.heldCollectibles;

        dto.heldItems = save.heldItems;
        dto.itemHolders = save.itemHolders;

        dto.openedDoors = save.openedDoors;

        dto.mineLocations = save.mineLocations;

        dto.frozenGuards = save.frozenGuards;
    }

    static void ReadMasterFromDto(MasterSave save, MasterSaveDto dto)
    {
        if(dto.currentLevelIndex is int currentLevelIndex)
        {
            save.currentLevelIndex = currentLevelIndex;
        }

        if(dto.difficulty is int difficulty)
        {
            save.difficulty = difficulty;
        }

        if (dto.visited is not null)
        {
            save.visited = new(dto.visited);
        }

        if(dto.levelStates is not null)
        {
            ReadLevelFromDto(save.levelStates[save.currentLevelIndex], dto.levelStates[save.currentLevelIndex]);
        }
    }

    static void ReadLevelFromDto(SaveState save, SaveStateDto dto)
    {
        //time
        if(dto.timeElapsed is float timeElapsed)
        {
            save.timeElapsed = timeElapsed;
        }

        //player position
        if(dto.playerPos is Vector2Int playerPos)
        {
            save.playerPos = playerPos;
        }

        //collectibles
        if(dto.heldCollectibles is not null)
        {
            save.heldCollectibles = new(dto.heldCollectibles);
        }

        //items
        if(dto.heldItems is not null)
        {
            save.heldItems = new(dto.heldItems);
        }

        if(dto.itemHolders is not null)
        {
            save.itemHolders = new(dto.itemHolders);
        }

        //doors
        if(dto.openedDoors is not null)
        {
            save.openedDoors = new(dto.openedDoors);
        }

        //mines
        if(dto.mineLocations is not null)
        {
            save.mineLocations = new(dto.mineLocations);
        }

        //frozen guards
        if(dto.frozenGuards is not null)
        {
            save.frozenGuards = new(dto.frozenGuards);
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
        //settings.Converters.Add( new MasterSaveConverter()                                      );
        settings.Converters.Add( new MasterSaveDtoConverter()                                   );
        return settings;
    }

    static string BuildPath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename);
    }
}
