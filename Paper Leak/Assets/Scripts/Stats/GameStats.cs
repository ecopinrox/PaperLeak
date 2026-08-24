using UnityEngine;
using System;
using UnityEditorInternal;

public class GameStats
{
    public float timeToBeat;
    public int saveCount;
    public bool clearedWithoutGettingCaught;
    public bool clearedWithoutChangingDifficulty;

    public GameStats(float timeToBeat, int saveCount, bool clearedWithoutGettingCaught, bool clearedWithoutChangingDifficulty)
    {
        this.timeToBeat = timeToBeat;
        this.saveCount = saveCount;
        this.clearedWithoutGettingCaught = clearedWithoutGettingCaught;
        this.clearedWithoutChangingDifficulty = clearedWithoutChangingDifficulty;
    }

    public void GetMinima(GameStats other)
    {
        if(!clearedWithoutChangingDifficulty)
        {
            timeToBeat = other.timeToBeat;
            saveCount = other.saveCount;
            clearedWithoutGettingCaught = other.clearedWithoutGettingCaught;
            clearedWithoutChangingDifficulty = other.clearedWithoutChangingDifficulty;
        }
        else
        {
            timeToBeat = Mathf.Min(other.timeToBeat, timeToBeat);
            saveCount = Mathf.Min(other.saveCount, saveCount);
            clearedWithoutGettingCaught |= other.clearedWithoutGettingCaught;
            clearedWithoutChangingDifficulty |= other.clearedWithoutChangingDifficulty;
        }
    }

    public string GetTimeString()
    {
        return GetTimeString(timeToBeat);
    }

    public static string GetTimeString(float time)
    {
        if(time < 0)
        {
            return "--:--:--";
        }

        return TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
    }
}
