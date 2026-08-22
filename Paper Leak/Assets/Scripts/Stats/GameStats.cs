using UnityEngine;

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
        timeToBeat = Mathf.Min(other.timeToBeat, timeToBeat);
        saveCount = Mathf.Min(other.saveCount, saveCount);
        clearedWithoutGettingCaught |= other.clearedWithoutGettingCaught;
        clearedWithoutChangingDifficulty |= other.clearedWithoutChangingDifficulty;
    }
}
