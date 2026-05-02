using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * DTO created to prevent older saves from breaking in newer updates.
 * 
 * See MasterSaveDto for more information.
 */

public class SaveStateDto
{
    public float? timeElapsed;

    public Vector2Int? playerPos;

    public HashSet<int> heldCollectibles;

    public List<ValueTuple<int, int>> heldItems;
    public Dictionary<Vector2Int, int> itemHolders;

    public HashSet<Vector2Int> openedDoors;

    public HashSet<Vector2Int> mineLocations;

    public Dictionary<Vector2Int, ValueTuple<Vector2Int, Quaternion>> frozenGuards;
}