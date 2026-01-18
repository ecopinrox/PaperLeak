using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save State", menuName = "Scriptable Objects/Save State")]
public class SaveState : ScriptableObject
{
    public Vector2Int playerPos;
}
