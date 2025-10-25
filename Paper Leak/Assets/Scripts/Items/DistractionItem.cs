using UnityEngine;

[RequireComponent(typeof(ItemPickup))]
public class DistractionItem : MonoBehaviour
{
    [SerializeField] int count;
    
    ItemPickup itemPickup;
    
    private void Awake()
    {
        itemPickup = GetComponent<ItemPickup>();
    }
    
    void Start()
    {
        itemPickup.OnPickup += (playerInventory) =>
        {
            playerInventory.IncrementPopPopCount(count);
            Destroy(gameObject);
        };
    }
}
