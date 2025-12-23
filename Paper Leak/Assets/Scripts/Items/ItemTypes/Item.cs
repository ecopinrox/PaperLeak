using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [field: SerializeField] public bool IsInfinite { get; protected set; } = false;
    [field: SerializeField] public int MaxStackSize { get; protected set; } = 9;

    public abstract void Use();
}
