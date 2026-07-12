using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPortraits", menuName = "Scriptable Objects/PortraitCollection")]
public class PortraitCollectionSO : ScriptableObject
{
    [SerializeField] string speakerName;
    [SerializeField] List<Sprite> portraits;

    public string Name { get { return speakerName; } }   
    public Sprite GetPortrait(int index) => portraits[index];
}
