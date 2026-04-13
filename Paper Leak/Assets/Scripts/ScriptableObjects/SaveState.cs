using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save State", menuName = "Scriptable Objects/Save State")]
public class SaveState : ScriptableObject
{
    //time
    public float timeElapsed;

    //player position
    public Vector2Int playerPos;

    //collectibles
    public HashSet<int> heldCollectibles = new();

    //items
    public List<ValueTuple<int, int>> heldItems = new();
    public Dictionary<Vector2Int, int> itemHolders = new();

    //doors
    public HashSet<Vector2Int> openedDoors = new();

    //mines
    public HashSet<Vector2Int> mineLocations = new();

    //frozen guards
    public Dictionary<Vector2Int, ValueTuple<Vector2Int, Quaternion>> frozenGuards = new();
}
