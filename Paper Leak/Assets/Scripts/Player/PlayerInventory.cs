using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    class ItemSlot
    {
        readonly ItemIndexer itemIndexer;

        public int ItemIndex { get; private set; }
        public int Count { get; private set; }
        public float RemainingCooldownSeconds { get; private set; }

        public GameObject ItemPrefab 
        { 
            get
            {
                return itemIndexer.GetItem(ItemIndex);
            }
        }

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

        public ItemSlot(int itemIndex, int count, ItemIndexer itemIndexer)
        {
            if(itemIndexer == null)
            {
                throw new NullReferenceException("Item indexer cannot be null");
            }
            this.itemIndexer = itemIndexer;

            ItemIndex = itemIndex;
            Count = count;
            RemainingCooldownSeconds = 0;
        }

        public ItemSlot(ItemIndexer itemIndexer) : this(-1, 0, itemIndexer)
        {

        }

        public void TickCooldown(float delta)
        {
            RemainingCooldownSeconds -= delta;

            if(RemainingCooldownSeconds <= 0)
            {
                RemainingCooldownSeconds = 0;
            }
        }

        public int AddItems(int itemIndex, int itemCount)
        {
            if(ItemPrefab == null)
            {
                ItemIndex = itemIndex;
            }

            if (itemIndex != ItemIndex)
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
            ItemIndex = -1;
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
            itemSlots[i] = new(LevelManager.Instance.ItemIndexer);
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
        //collectibles
        saveState.heldCollectibles = new(HeldCollectibles);

        //items
        saveState.heldItems = new();
        for(int i = 0; i < slotCount; i++)
        {
            saveState.heldItems.Add(new ValueTuple<int, int>(itemSlots[i].ItemIndex, itemSlots[i].Count));
        }
    }

    void LoadInventory(SaveState saveState)
    {
        //collectibles
        HeldCollectibles = new(saveState.heldCollectibles);

        uiManager.UpdateCollectibleIcons(HeldCollectibles);

        //items
        for(int i = 0; i < slotCount; i++)
        {
            if(i >= saveState.heldItems.Count)
            {
                itemSlots[i] = new(LevelManager.Instance.ItemIndexer);
            }
            else
            {
                itemSlots[i] = new(saveState.heldItems[i].Item1, saveState.heldItems[i].Item2, LevelManager.Instance.ItemIndexer); 
            }
        }

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
    /// <param name="itemIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int AddItems(int itemIndex, int count)
    {
        if(itemIndex < 0)
        {
            throw new NullReferenceException("Attempted to add a null item prefab to inventory.");
        }

        //find a slot containing itemPrefab - if found, insert and return
        for(int i = 0; i < slotCount; i++)
        {
            if (itemSlots[i].ItemIndex != itemIndex)
            {
                continue;
            }

            int itemsAdded = itemSlots[i].AddItems(itemIndex, count);
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

            int itemsAdded = itemSlots[i].AddItems(itemIndex, count);
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
