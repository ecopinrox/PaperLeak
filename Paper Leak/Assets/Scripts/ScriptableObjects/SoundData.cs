using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SoundData", menuName = "Scriptable Objects/Sound Data")]
public class SoundData : ScriptableObject
{
    public List<AudioClip> audioClips;
    public float distractionRadius;
    public int priority;
}
