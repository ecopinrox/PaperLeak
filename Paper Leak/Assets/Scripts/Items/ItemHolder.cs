using UnityEngine;

[RequireComponent(typeof(Pickup))]
[ExecuteAlways]
public class ItemHolder : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] int itemCount = 1;

    [SerializeField] SpriteRenderer spriteRenderer;

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

        SpriteRenderer itemSpriteRenderer = itemPrefab.GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sprite = itemSpriteRenderer.sprite;
        spriteRenderer.color = itemSpriteRenderer.color;
    }
}
