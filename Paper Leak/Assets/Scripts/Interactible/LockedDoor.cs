using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Interactible
{
    [SerializeField] int keyID;

    GridManager gridManager;
    Pathfinder pathfinder;

    public Vector2Int GridPos { get { return Vector2Int.RoundToInt(transform.position); } }

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        pathfinder = FindFirstObjectByType<Pathfinder>();
    }

    public override void Interact(out bool uiEnabled)
    {
        uiEnabled = false;

        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (playerInventory.HasCollectible(keyID))
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        List<Vector2Int> tiles = GetWallTiles();

        foreach(Vector2Int tile in tiles)
        {
            gridManager.RemoveWallTile(tile);
        }

        pathfinder.AddDoorTilesToRegionList(tiles);

        Destroy(gameObject);
    }

    List<Vector2Int> GetWallTiles()
    {
        List<Vector2Int> tiles = new();

        int width = Mathf.RoundToInt(transform.localScale.x);
        int height = Mathf.RoundToInt(transform.localScale.y);

        Vector2 bottomLeft = (Vector2)transform.position - new Vector2(width / 2f - 0.5f, height / 2f - 0.5f);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2Int tile = Vector2Int.RoundToInt(bottomLeft + new Vector2(i, j));
                tiles.Add(tile);
            }
        }
        
        return tiles;
    }
}
