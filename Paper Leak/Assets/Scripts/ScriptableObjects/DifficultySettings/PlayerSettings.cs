using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/Settings/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public SoundData walkSFX;
    public SoundData paperTearingSFX;

    public float walkingViewDistanceMultiplier = 1f;
    public float crawlingViewDistanceMultiplier = 0.75f;
}
