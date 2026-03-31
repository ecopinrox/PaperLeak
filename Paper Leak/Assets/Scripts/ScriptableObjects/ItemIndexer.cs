using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemIndexer", menuName = "Scriptable Objects/ItemIndexer")]
public class ItemIndexer : ScriptableObject
{
    [SerializeField] List<GameObject> items;

    public GameObject GetItem(int index)
    {
        if(index >= items.Count)
        {
            throw new ArgumentOutOfRangeException($"No item has index {index}");
        }

        return (index < 0) ? null : items[index];
    }
}
