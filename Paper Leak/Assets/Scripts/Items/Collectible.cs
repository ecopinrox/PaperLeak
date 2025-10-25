using UnityEngine;

[RequireComponent(typeof(ItemPickup))]
public class Collectible : MonoBehaviour
{
    [SerializeField] int collectibleID;
    
    ItemPickup itemPickup;

    private void Awake()
    {
        itemPickup = GetComponent<ItemPickup>();
    }

    private void Start()
    {
        itemPickup.OnPickup += (playerInventory) =>
        {
            playerInventory.AddObject(collectibleID);
            Destroy(gameObject);
        };
    }
}
