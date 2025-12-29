using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    struct ItemSlot
    {
        public GameObject ItemPrefab { get; private set; }
        public int Count { get; private set; }
        public float RemainingCooldownSeconds { get; private set; }

        public readonly bool IsInfinite 
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

        public readonly int MaxStackSize 
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

        readonly float MaxCooldownSeconds 
        { 
            get
            {
                return ItemPrefab.GetComponent<Item>().CooldownSeconds;
            }
        }

        public readonly float CooldownFraction
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

        public void TickCooldown(float delta)
        {
            RemainingCooldownSeconds -= delta;

            if(RemainingCooldownSeconds <= 0)
            {
                RemainingCooldownSeconds = 0;
            }
        }

        public bool TryAddItems(GameObject item, int itemCount, out int itemsAdded)
        {
            itemsAdded = 0;

            if(ItemPrefab == null)
            {
                ItemPrefab = item;
            }

            if (item != ItemPrefab)
            {
                return false;
            }

            int space = MaxStackSize - Count;

            if(space <= 0)
            {
                return false;
            }

            Count = Mathf.Min(Count + itemCount, MaxStackSize);
            itemsAdded = Mathf.Min(itemCount, space);
            return true;
        }

        public void UseItem()
        {
            if (ItemPrefab == null)
            {
                return;
            }

            if (RemainingCooldownSeconds > 0)
            {
                return;
            }

            ItemPrefab.GetComponent<Item>().Use();
            RemainingCooldownSeconds = MaxCooldownSeconds;

            DecrementCount();
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

    public HashSet<int> Collectibles { get; private set; } = new();

    [SerializeField] int slotCount = 6;
    ItemSlot[] itemSlots;
    int selectedItemSlot = 0;

    UIManager uiManager;

    private void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();

        LevelManager.OnLoadState += LoadInventory;

        itemSlots = new ItemSlot[slotCount];
    }

    private void Start()
    {
        UpdateItemSlotUI();
    }

    private void FixedUpdate()
    {
        TickItemCooldowns(Time.fixedDeltaTime);
    }

    private void OnDisable()
    {
        LevelManager.OnLoadState -= LoadInventory;
    }

    void LoadInventory()
    {
        Collectibles = new(LevelManager.SaveState.collectibles);

        uiManager.UpdateCollectibleIcons(Collectibles);
        UpdateItemSlotUI();
    }

    #region Collectibles

    public void AddCollectible(int id)
    {
        Collectibles.Add(id);

        uiManager.UpdateCollectibleIcons(Collectibles);
    }

    public bool HasCollectible(int id)
    {
        if (Collectibles.Contains(id))
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

    public void UseSelectedItem()
    {
        itemSlots[selectedItemSlot].UseItem();
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

        for(int i = 0; i < slotCount; i++)
        {
            if(!itemSlots[i].TryAddItems(itemPrefab, count, out int itemsAdded))
            {
                continue;
            }

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
