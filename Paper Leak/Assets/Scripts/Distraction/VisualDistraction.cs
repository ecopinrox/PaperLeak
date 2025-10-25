using UnityEngine;

public class VisualDistraction : Distraction 
{
    [field: SerializeField] public float ViewDistanceMultiplier { get; private set; } = 1f;

    protected override void OnEnable()
    {
        base.OnEnable();

        GuardDistractionSensor[] guards = FindObjectsByType<GuardDistractionSensor>(FindObjectsSortMode.None);
        foreach (GuardDistractionSensor guard in guards)
            guard.AddVisualDistraction(this);
    }

    public void SetPosition(Vector2 pos)
    {
        getPosition = () => Vector2Int.RoundToInt(pos);
    }

    public void SetViewDistanceMultiplier(float multiplier)
    {
        ViewDistanceMultiplier = multiplier;
    }
}
