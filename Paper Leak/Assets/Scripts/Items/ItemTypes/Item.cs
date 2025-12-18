using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [field: SerializeField] public int MaxStackSize { get; protected set; } = 9;

    public abstract void Use();
}
