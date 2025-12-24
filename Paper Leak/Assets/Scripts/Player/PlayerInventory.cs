using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    struct ItemSlot
    {
        public GameObject ItemPrefab { get; private set; }
        public int Count { get; private set; }

        readonly bool IsInfinite 
        { 
            get 
            { 
                return ItemPrefab.GetComponent<Item>().IsInfinite; 
            } 
        }

        readonly int MaxStackSize 
        { 
            get 
            {
                if (IsInfinite)
                {
                    return 1;
                }

                return ItemPrefab.GetComponent<Item>().MaxStackSize; 
            } 
        }

        public ItemSlot(GameObject itemPrefab, int count)
        {
            ItemPrefab = itemPrefab;
            Count = count;
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

            ItemPrefab.GetComponent<Item>().Use();
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
            Debug.Log("Slot empty");
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
        UpdateItemUI();
    }

    private void OnDisable()
    {
        LevelManager.OnLoadState -= LoadInventory;
    }

    void LoadInventory()
    {
        Collectibles = new(LevelManager.SaveState.collectibles);

        uiManager.UpdateCollectibleIcons(Collectibles);
        UpdateItemUI();
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
        UpdateItemUI();
    }

    public void UseSelectedItem()
    {
        itemSlots[selectedItemSlot].UseItem();
        UpdateItemUI();
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

            UpdateItemUI();
            return itemsAdded;
        }

        UpdateItemUI();
        return 0;
    }

    void UpdateItemUI()
    {
        for(int i = 0; i < slotCount; i++)
        {
            ItemSlot current = itemSlots[i];
            uiManager.UpdateItemSlot(current.ItemPrefab, current.Count, i);
        }
        uiManager.SelectItemSlot(selectedItemSlot);
    }

    #endregion
}
