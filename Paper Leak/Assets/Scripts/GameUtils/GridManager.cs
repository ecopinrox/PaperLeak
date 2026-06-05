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
    [SerializeField] Tilemap moveBlockingTilemap;

    /// <summary>
    /// Checks if the next tile (given by <paramref name="currentLoc"/> and <paramref name="direction"/>) can currently be moved to. Does not ignore MoveBlockingTilemap.
    /// </summary>
    /// <param name="currentLoc"></param>
    /// <param name="direction"></param>
    /// <param name="isCrawling"></param>
    /// <returns></returns>
    public bool CanMove(Vector3 currentLoc, Vector2 direction, bool isCrawling)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(currentLoc + (Vector3)direction);
        if (!groundTilemap.HasTile(gridPosition)
            || wallTilemap.HasTile(gridPosition)
            || glassTilemap.HasTile(gridPosition)
            || moveBlockingTilemap.HasTile(gridPosition)
            || (!isCrawling && crawlTilemap.HasTile(gridPosition))) 
            return false;
        return true;
    }

    /// <summary>
    /// Checks if the tile at <paramref name="loc"/> can be moved to. Ignores MoveBlockingTilemap.
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
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
        if (((mask.value & 1 << groundTilemap.gameObject.layer) != 0)       && groundTilemap        .HasTile(gridPosition)) return true;
        if (((mask.value & 1 << crawlTilemap.gameObject.layer) != 0)        && crawlTilemap         .HasTile(gridPosition)) return true;
        if (((mask.value & 1 << wallTilemap.gameObject.layer) != 0)         && wallTilemap          .HasTile(gridPosition)) return true;
        if (((mask.value & 1 << glassTilemap.gameObject.layer) != 0)        && glassTilemap         .HasTile(gridPosition)) return true;
        if (((mask.value & 1 << moveBlockingTilemap.gameObject.layer) != 0) && moveBlockingTilemap  .HasTile(gridPosition)) return true;
        return false;
    }

    public void RemoveWallTile(Vector2Int loc)
    {
        wallTilemap.SetTile((Vector3Int)loc - new Vector3Int(1,1,0), null);
    }
}

