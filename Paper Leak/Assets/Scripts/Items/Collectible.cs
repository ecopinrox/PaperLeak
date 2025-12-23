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

    private void Start()
    {
        pickup.OnPickup += (playerInventory) =>
        {
            playerInventory.AddCollectible(collectibleID);
            Destroy(gameObject);
        };
    }
}
