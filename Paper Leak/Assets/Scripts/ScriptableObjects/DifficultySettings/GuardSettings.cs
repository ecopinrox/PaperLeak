using UnityEngine;

[CreateAssetMenu(fileName = "GuardSettings", menuName = "Scriptable Objects/Settings/GuardSettings")]
public class GuardSettings : ScriptableObject
{
    public float investigationRoutineDelay = 1f;
    public float examineDelay = 6f;

    [Range(0f, 180f)] public float frontalViewAngle = 30f;
    [Range(0f, 180f)] public float peripheralViewAngle = 75f;
    public float frontalViewRadius = 7f;
    public float peripheralViewRadius = 4f;
    [Range(0f, 1f)] public float dangerZoneMultiplier = 0.6f;
    public float soundAlertDistanceMultiplier;
    public bool canCrouch = false;

    public float patrolSpeed = 1f;
}
