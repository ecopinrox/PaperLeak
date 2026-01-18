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
            itemCount = 0;
            gameObject.SetActive(false);
        }
    }

    void AssignSprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if(itemPrefab == null)
        {
            return;
        }

        SpriteRenderer itemSpriteRenderer = itemPrefab.GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.sprite = itemSpriteRenderer.sprite;
        spriteRenderer.color = itemSpriteRenderer.color;
    }

    void LoadState(SaveState saveState)
    {
        if(saveState.itemHolders.TryGetValue(pickup.GridPos, out int count))
        {
            itemCount = count;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void SaveState(SaveState saveState)
    {
        if(gameObject.activeSelf)
        {
            if(saveState.itemHolders.ContainsKey(pickup.GridPos))
            {
                saveState.itemHolders[pickup.GridPos] = itemCount;
            }
            else
            {
                saveState.itemHolders.Add(pickup.GridPos, itemCount);
            }
        }
        else
        {
            saveState.itemHolders.Remove(pickup.GridPos);
        }
    }
}
