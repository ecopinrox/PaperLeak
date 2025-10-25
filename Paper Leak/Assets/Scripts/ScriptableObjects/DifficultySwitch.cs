using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultySwitch", menuName = "Scriptable Objects/DifficultySwitch")]
public class DifficultySwitch : ScriptableObject
{
    public static Action<int> loadDifficultySettings;

    public List<ScriptableObject> settingsList;

    public T GetDifficultySettings<T>(int level) where T : ScriptableObject
    {
        return (T)settingsList[level];
    }
}
