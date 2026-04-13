using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "MasterSave", menuName = "Scriptable Objects/MasterSave")]
public class MasterSave : ScriptableObject
{
    public int currentLevelIndex;
    public int difficulty;
    public HashSet<int> visited = new();
    public SaveState[] levelStates;

    public SaveState GetCurrentLevelState() => levelStates[currentLevelIndex]; 
}
