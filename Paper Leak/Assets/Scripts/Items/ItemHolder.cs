using UnityEngine;

[RequireComponent(typeof(ItemPickup))]
public class ItemHolder : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] int itemCount = 1;

    ItemPickup itemPickup;

    private void Awake()
    {
        itemPickup = GetComponent<ItemPickup>();
    }

    private void Start()
    {
        itemPickup.OnPickup += AddToInventory;
    }

    void AddToInventory(PlayerInventory playerInventory)
    {
        //unimplemented
    }
}
