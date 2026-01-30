using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Pickup))]
public class Landmine : MonoBehaviour
{
    [SerializeField] GameObject LandmineSetterPrefab;
    [SerializeField] SoundData freezeSFX;

    Pickup pickup;

    private void Awake()
    {
        pickup = GetComponent<Pickup>();
    }

    private void OnEnable()
    {
        LevelManager.OnStateSave += SaveState;
    }

    private void OnDestroy()
    {
        LevelManager.OnStateSave -= SaveState;
    }

    private void Start()
    {
        pickup.OnPickup += ReturnToInventory;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out GuardBrain guardBrain))
        {
            guardBrain.Freeze();
            SoundManager.Instance.PlaySound(freezeSFX, transform.position, null);
            gameObject.SetActive(false);
        }
    }

    private void ReturnToInventory(PlayerInventory playerInventory)
    {
        playerInventory.AddItems(LandmineSetterPrefab, 1);

        gameObject.SetActive(false);
    }

    void SaveState(SaveState saveState)
    {
        if(!gameObject.activeSelf)
        {
            saveState.mineLocations.Remove(pickup.GridPos);
        }
        else if (!saveState.mineLocations.Contains(pickup.GridPos))
        {
            saveState.mineLocations.Add(pickup.GridPos);
        }
    }
}
