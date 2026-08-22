using UnityEngine;

[CreateAssetMenu(fileName = "SFXVolume", menuName = "Scriptable Objects/SFXVolume")]
public class SFXVolume : ScriptableObject
{
    [Range(0f, 1f)]public float volume;
}
