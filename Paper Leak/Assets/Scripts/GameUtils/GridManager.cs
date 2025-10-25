using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap crawlTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap glassTilemap;

    private void Awake()
    {
        //Debug.Log("-24, 9: " + IsLocationInMask(new(-24, 9), LayerMask.GetMask("Crawlable")));
    }

    public bool CanMove(Vector3 currentLoc, Vector2 direction, bool isCrawling)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(currentLoc + (Vector3)direction);
        if (!groundTilemap.HasTile(gridPosition)
            || wallTilemap.HasTile(gridPosition)
            || glassTilemap.HasTile(gridPosition)
            || (!isCrawling && crawlTilemap.HasTile(gridPosition))) 
            return false;
        return true;
    }

    public bool IsWalkable(Vector2Int loc)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell((Vector2)loc);
        if (!groundTilemap.HasTile(gridPosition)
            || wallTilemap.HasTile(gridPosition)
            || crawlTilemap.HasTile(gridPosition)
            || glassTilemap.HasTile(gridPosition))
            return false;
        return true;
    }

    public bool IsLocationInMask(Vector2Int loc, LayerMask mask)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell((Vector2)loc);
        if (((mask.value & 1 << groundTilemap.gameObject.layer) != 0)   && groundTilemap.HasTile(gridPosition)) return true;
        if (((mask.value & 1 << crawlTilemap.gameObject.layer) != 0)    && crawlTilemap .HasTile(gridPosition)) return true;
        if (((mask.value & 1 << wallTilemap.gameObject.layer) != 0)     && wallTilemap  .HasTile(gridPosition)) return true;
        if (((mask.value & 1 << glassTilemap.gameObject.layer) != 0)    && glassTilemap .HasTile(gridPosition)) return true;
        return false;
    }

    public void RemoveWallTile(Vector2Int loc)
    {
        wallTilemap.SetTile((Vector3Int)loc - new Vector3Int(1,1,0), null);
    }
}
