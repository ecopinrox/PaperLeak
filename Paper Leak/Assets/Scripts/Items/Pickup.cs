using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Pickup : MonoBehaviour
{
    public event Action<PlayerInventory> OnPickup;
    Vector2Int GridPos => Vector2Int.RoundToInt(transform.position);

    private void Awake()
    {
        LevelManager.OnLoadState += CheckIfItemAcquired;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerInventory playerInventory))
        {
            MarkItemAsCollected();
            OnPickup?.Invoke(playerInventory);
        }
    }

    void OnDisable()
    {
        LevelManager.OnLoadState -= CheckIfItemAcquired;
    }

    void CheckIfItemAcquired()
    {
        if (LevelManager.SaveState.collectedItemLocations.Contains(GridPos))
            Destroy(gameObject);
    }

    void MarkItemAsCollected()
    {
        LevelManager.Instance.collectedItemLocations.Add(GridPos);
    }
}
