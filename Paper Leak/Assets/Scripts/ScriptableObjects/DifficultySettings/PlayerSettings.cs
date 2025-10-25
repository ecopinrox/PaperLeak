using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/Settings/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public SoundData walkSFX;
    public SoundData paperTearingSFX;
}
