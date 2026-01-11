using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Distraction : MonoBehaviour
{
    [field: SerializeField] public int Priority { get; protected set; }
    public Vector2Int Position { get { return getPosition(); } }
    protected Func<Vector2Int> getPosition;

    protected virtual void OnEnable()
    {
        getPosition = () => Vector2Int.RoundToInt(transform.position);
    }
}

    