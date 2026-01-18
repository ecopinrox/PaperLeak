using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save State", menuName = "Scriptable Objects/Save State")]
public class SaveState : ScriptableObject
{
    //player position
    public Vector2Int playerPos;

    //player inventory
    public HashSet<int> heldCollectibles = new();
}
