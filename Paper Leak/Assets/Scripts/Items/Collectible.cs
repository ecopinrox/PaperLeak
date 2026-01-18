using UnityEngine;

[RequireComponent(typeof(Pickup))]
public class Collectible : MonoBehaviour
{
    [SerializeField] int collectibleID;
    
    Pickup pickup;

    private void Awake()
    {
        pickup = GetComponent<Pickup>();
    }

    private void OnEnable()
    {
        LevelManager.OnStateLoad += LoadState;
        LevelManager.OnStateSave += SaveState;
    }

    private void OnDestroy()
    {
        LevelManager.OnStateLoad -= LoadState;
        LevelManager.OnStateSave -= SaveState;
    }

    private void Start()
    {
        pickup.OnPickup += (playerInventory) =>
        {
            playerInventory.AddCollectible(collectibleID);
            gameObject.SetActive(false);
        };
    }

    void LoadState(SaveState saveState)
    {
        if(saveState.heldCollectibles.Contains(collectibleID))
        {
            gameObject.SetActive(false);
        }
    }

    void SaveState(SaveState saveState)
    {
        if(gameObject.activeSelf && !saveState.heldCollectibles.Contains(collectibleID))
        {
            saveState.heldCollectibles.Add(collectibleID);
        }
        else if(!gameObject.activeSelf && saveState.heldCollectibles.Contains(collectibleID))
        {
            saveState.heldCollectibles.Remove(collectibleID);
        }
    }
}
