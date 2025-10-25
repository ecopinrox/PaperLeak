using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] float waitTime;
    public Vector2Int Position { get { return new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)); } }
    public Quaternion Rotation { get { return transform.rotation; } }
    public float WaitTime { get { return waitTime; } }
}
