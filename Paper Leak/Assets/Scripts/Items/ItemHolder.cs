using UnityEngine;

[RequireComponent(typeof(Pickup))]
[ExecuteAlways]
public class ItemHolder : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] int itemCount = 1;

    [SerializeField] SpriteRenderer sprite;

    Pickup pickup;

    private void Awake()
    {
        pickup = GetComponent<Pickup>();
    }

    private void Start()
    {
        pickup.OnPickup += AddToInventory;
    }

    private void OnValidate()
    {
        AssignSprite();
    }

    void AddToInventory(PlayerInventory playerInventory)
    {
        int itemsAdded = playerInventory.AddItems(itemPrefab, itemCount);
        Debug.Log(itemsAdded + " items added");

        itemCount -= itemsAdded;
        if(itemCount <= 0)
        {
            Destroy(gameObject);
        }
    }

    void AssignSprite()
    {
        if(itemPrefab == null)
        {
            return;
        }

        sprite.sprite = itemPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
    }
}
