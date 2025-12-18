using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public HashSet<int> Collectibles { get; private set; } = new();

    UIManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<UIManager>();

        LevelManager.OnLoadSave += LoadInventory;
    }

    private void OnDisable()
    {
        LevelManager.OnLoadSave -= LoadInventory;
    }

    void LoadInventory()
    {
        Collectibles = new(LevelManager.SaveState.collectibles);

        gameManager.UpdateCollectibleIcons(Collectibles);
    }

    public void AddObject(int id)
    {
        Collectibles.Add(id);

        gameManager.UpdateCollectibleIcons(Collectibles);
    }

    public bool HasObject(int id)
    {
        if (Collectibles.Contains(id))
        {
            return true;
        }
        return false;
    }
}
