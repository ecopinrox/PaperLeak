using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickup : MonoBehaviour
{
    public event Action<PlayerInventory> OnPickup;
    public Vector2Int GridPos => Vector2Int.RoundToInt(transform.position);

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerInventory playerInventory))
        {
            OnPickup?.Invoke(playerInventory);
        }
    }
}
