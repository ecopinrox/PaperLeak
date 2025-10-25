using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public HashSet<int> Items { get; private set; } = new();
    [field: SerializeField] public int PaperBallCount { get; private set; } = 3;
    [field: SerializeField] public int PopPopCount { get; private set; } = 3;
    [SerializeField] int paperBallLimit = 3;

    UIManager gameManager;
    PlayerDistraction playerDistraction;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<UIManager>();
        playerDistraction = GetComponent<PlayerDistraction>();

        LevelManager.OnLoadSave += LoadInventory;
    }

    private void OnDisable()
    {
        LevelManager.OnLoadSave -= LoadInventory;
    }

    void LoadInventory()
    {
        Items = new(LevelManager.SaveState.items);
        PaperBallCount = LevelManager.SaveState.paperBallCount;
        PopPopCount = LevelManager.SaveState.popPopCount;

        gameManager.UpdateCollectibleIcons(Items);
        playerDistraction.UpdateDistractionInformation();
    }

    public void AddObject(int id)
    {
        Items.Add(id);

        gameManager.UpdateCollectibleIcons(Items);
    }

    public bool HasObject(int id)
    {
        if (Items.Contains(id))
        {
            return true;
        }
        return false;
    }

    public void DecrementPaperBallCount() => PaperBallCount--;
    public void IncrementPaperBallCount(int count)
    {
        PaperBallCount += count;
        if(PaperBallCount > paperBallLimit) PaperBallCount = paperBallLimit;
        playerDistraction.UpdateDistractionInformation();
    }
    public void DecrementPopPopCount() => PopPopCount--;
    public void IncrementPopPopCount(int count) 
    {
        PopPopCount += count;
        playerDistraction.UpdateDistractionInformation();
    }
}
