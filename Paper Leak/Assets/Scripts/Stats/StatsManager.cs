using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class StatsManager
{
    //2 types of stats files
    //  1. session stats - contains stats for the current save
    //  2. overall stats - contains minimum (best) stats so far

    public static bool CheckForStats(string filename)
    {
        return File.Exists(BuildPath(filename));
    }

    public static GameStats ReadStats(string filename)
    {
        if(!CheckForStats(filename))
        {
            return null;
        }

        string json;
        using (StreamReader reader = new(BuildPath(filename)))
        {
            json = reader.ReadToEnd();
        }

        return JsonConvert.DeserializeObject<GameStats>(json);
    }

    public static void WriteStats(string filename, GameStats stats)
    {
        string json = JsonConvert.SerializeObject(stats, Formatting.Indented);
        using(StreamWriter writer = new(BuildPath(filename)))
        {
            writer.Write(json);
        }
    }

    public static void UpdateOverallStats(string filename, GameStats stats)
    {
        if(CheckForStats(filename))
        {
            GameStats old = ReadStats(filename);
            stats.GetMinima(old);
        }

        WriteStats(filename, stats);
    }

    static string BuildPath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename);
    }
}
