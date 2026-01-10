using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [field: SerializeField] public bool IsInfinite { get; protected set; } = false;
    [field: SerializeField] public int MaxStackSize { get; protected set; } = 9;
    [field: SerializeField] public float CooldownSeconds { get; protected set; } = 1f;

    /// <summary>
    /// </summary>
    /// <returns>A boolean indicating whether the item was successfully used or not.</returns>
    public abstract Awaitable<bool> Use();
}
