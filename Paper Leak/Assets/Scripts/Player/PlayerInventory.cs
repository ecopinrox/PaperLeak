using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    class ItemSlot
    {
        public GameObject ItemPrefab { get; private set; }
        public int Count { get; private set; }
        public float RemainingCooldownSeconds { get; private set; }

        public bool IsInfinite 
        { 
            get 
            { 
                if(ItemPrefab == null)
                {
                    return true;
                }

                return ItemPrefab.GetComponent<Item>().IsInfinite; 
            } 
        }

        public int MaxStackSize 
        { 
            get 
            {
                if(ItemPrefab == null)
                {
                    return 0;
                }

                if (IsInfinite)
                {
                    return 1;
                }

                return ItemPrefab.GetComponent<Item>().MaxStackSize; 
            } 
        }

        float MaxCooldownSeconds 
        { 
            get
            {
                return (ItemPrefab == null) ? 0 : ItemPrefab.GetComponent<Item>().CooldownSeconds;
            }
        }

        public float CooldownFraction
        {
            get
            {
                if(ItemPrefab == null)
                {
                    return 0;
                }

                if(MaxCooldownSeconds == 0)
                {
                    return 0;
                }

                return RemainingCooldownSeconds / MaxCooldownSeconds;
            }
        }

        public ItemSlot()
        {
            ItemPrefab = null;
            Count = 0;
            RemainingCooldownSeconds = 0;
        }

        public void TickCooldown(float delta)
        {
            RemainingCooldownSeconds -= delta;

            if(RemainingCooldownSeconds <= 0)
            {
                RemainingCooldownSeconds = 0;
            }
        }

        public int AddItems(GameObject item, int itemCount)
        {
            if(ItemPrefab == null)
            {
                ItemPrefab = item;
            }

            if (item != ItemPrefab)
            {
                return 0;
            }

            int space = MaxStackSize - Count;

            if(space <= 0)
            {
                return 0;
            }

            Count = Mathf.Min(Count + itemCount, MaxStackSize);
            return Mathf.Min(itemCount, space);
        }

        public async Awaitable UseItem()
        {
            if (ItemPrefab == null)
            {
                return;
            }

            if (RemainingCooldownSeconds > 0)
            {
                return;
            }

            if(await ItemPrefab.GetComponent<Item>().Use())
            {
                DecrementCount();
                RemainingCooldownSeconds = MaxCooldownSeconds;
            }
        }

        void DecrementCount()
        {
            if (IsInfinite)
            {
                return;
            }

            Count--;
            if(Count <= 0)
            {
                ClearSlot();
            }
        }

        public void ClearSlot()
        {
            ItemPrefab = null;
            Count = 0;
            RemainingCooldownSeconds = 0;
        }
    }

    public static PlayerInventory Instance { get; private set; }

    public HashSet<int> HeldCollectibles { get; private set; } = new();

    [SerializeField] int slotCount = 6;
    ItemSlot[] itemSlots;
    int selectedItemSlot = 0;

    UIManager uiManager;

    private void Awake()
    {
        Instance = this;

        uiManager = FindFirstObjectByType<UIManager>();

        itemSlots = new ItemSlot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            itemSlots[i] = new();
        }
    }

    private void OnEnable()
    {
        LevelManager.OnStateLoad += LoadInventory;
        LevelManager.OnStateSave += SaveInventory;
    }

    private void OnDisable()
    {
        LevelManager.OnStateLoad -= LoadInventory;
        LevelManager.OnStateSave -= SaveInventory;
    }

    private void Start()
    {
        UpdateItemSlotUI();
    }

    private void FixedUpdate()
    {
        TickItemCooldowns(Time.fixedDeltaTime);
    }

    void SaveInventory(SaveState saveState)
    {
        saveState.heldCollectibles = new(HeldCollectibles);
    }

    void LoadInventory(SaveState saveState)
    {
        HeldCollectibles = new(saveState.heldCollectibles);

        uiManager.UpdateCollectibleIcons(HeldCollectibles);
        UpdateItemSlotUI();
    }

    #region Collectibles

    public void AddCollectible(int id)
    {
        HeldCollectibles.Add(id);

        uiManager.UpdateCollectibleIcons(HeldCollectibles);
    }

    public bool HasCollectible(int id)
    {
        if (HeldCollectibles.Contains(id))
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Items

    public void SelectItemSlot(int slotIndex)
    {
        selectedItemSlot = slotIndex;
        UpdateItemSlotUI();
    }

    public async Awaitable UseSelectedItem()
    {
        await itemSlots[selectedItemSlot].UseItem();

        UpdateItemSlotUI();
    }

    /// <summary>
    /// Returns the number of items added.
    /// </summary>
    /// <param name="itemPrefab"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int AddItems(GameObject itemPrefab, int count)
    {
        if(itemPrefab == null)
        {
            throw new NullReferenceException("Attempted to add a null item prefab to inventory.");
        }

        //find a slot containing itemPrefab - if found, insert and return
        for(int i = 0; i < slotCount; i++)
        {
            if (itemSlots[i].ItemPrefab != itemPrefab)
            {
                continue;
            }

            int itemsAdded = itemSlots[i].AddItems(itemPrefab, count);
            UpdateItemSlotUI();

            return itemsAdded;
        }

        //find an empty slot - if found, insert and return
        for(int i = 0; i < slotCount; i++)
        {
            if (itemSlots[i].ItemPrefab != null)
            {
                continue;
            }

            int itemsAdded = itemSlots[i].AddItems(itemPrefab, count);
            UpdateItemSlotUI();

            return itemsAdded;
        }

        UpdateItemSlotUI();
        return 0;
    }

    private void TickItemCooldowns(float delta)
    {
        for(int i = 0; i < slotCount; i++)
        {
            itemSlots[i].TickCooldown(delta);
        }
        UpdateCooldownUI();
    }

    void UpdateItemSlotUI()
    {
        for(int i = 0; i < slotCount; i++)
        {
            ItemSlot current = itemSlots[i];
            uiManager.UpdateItemSlot(current.ItemPrefab, current.Count, current.IsInfinite, i);
        }
        uiManager.SelectItemSlot(selectedItemSlot);
    }

    void UpdateCooldownUI()
    {
        for(int i = 0; i < slotCount; i++)
        {
            uiManager.UpdateCooldownOverlay(itemSlots[i].CooldownFraction, i);
        }
    }    

    #endregion
}
