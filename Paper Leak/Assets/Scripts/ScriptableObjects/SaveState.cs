using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save State", menuName = "Scriptable Objects/Save State")]
public class SaveState : ScriptableObject
{
    public Vector2Int playerPos;
    public HashSet<int> collectibles = new();
    public HashSet<Vector2Int> openedDoors = new();
    public HashSet<Vector2Int> collectedItemLocations = new();
    public float timeElapsed = 0;
}
