using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save State", menuName = "Scriptable Objects/Save State")]
public class SaveState : ScriptableObject
{
    //player position
    public Vector2Int playerPos;

    //collectibles
    public HashSet<int> heldCollectibles = new();

    //items
    public List<ValueTuple<GameObject, int>> heldItems = new();
    public Dictionary<Vector2Int, int> itemHolders = new();

    //doors
    public HashSet<Vector2Int> openedDoors = new();

    //mines

    //frozen guards
}
