using System;
using System.Collections.Generic;
using System.Diagnostics;

public class IndexedSet<T>
{
    readonly Dictionary<int, HashSet<T>> setDictionary = new();

    public int HighestIndexUsed { get; private set; } = -1;

    public int Count 
    { 
        get 
        { 
            int count = 0;
            foreach (HashSet<T> set in setDictionary.Values)
            {
                count += set.Count;
            }
            return count;
        } 
    }

    public HashSet<T> this[int index]
    {
        get 
        { 
            return setDictionary[index]; 
        }
    }

    public void Add(T item, int index)
    {
        Remove(item);

        if(setDictionary.ContainsKey(index))
        {
            setDictionary[index].Add(item);
        }
        else
        {
            setDictionary.Add(index, new() { item });
        }

        if(index > HighestIndexUsed) HighestIndexUsed = index;
    }

    public void Merge(int indexA, int indexB)
    {
        HashSet<T> setA = setDictionary[indexA];
        HashSet<T> setB = setDictionary[indexB];
        
        setA.UnionWith(setB);
        setDictionary.Remove(indexB);
    }

    public bool Contains(T item)
    {
        foreach(HashSet<T> set in setDictionary.Values)
        {
            if (set.Contains(item))
            { 
                return true; 
            }
        }
        return false;
    }

    public int GetIndex(T item)
    {
        foreach(int i in setDictionary.Keys) 
        {
            if (setDictionary[i].Contains(item))
            {
                return i;
            }
        }

        throw new KeyNotFoundException($"The specified item {item} does not exist in the indexed set.");
    }

    public bool TryGetIndex(T item, out int index)
    {
        foreach(int i in setDictionary.Keys)
        {
            if (setDictionary[i].Contains(item))
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    public void Remove(T item)
    {
        if(!TryGetIndex(item, out int index))
        {
            return;
        }

        setDictionary[index].Remove(item);
        if (setDictionary[index].Count == 0) 
        { 
            setDictionary.Remove(index); 
        }
    }

    public void Clear()
    {
        setDictionary.Clear();
    }

    public T PopItem(int index)
    {
        IEnumerator<T> enumerator = setDictionary[index].GetEnumerator();
        if(!enumerator.MoveNext())
        {
            throw new IndexOutOfRangeException($"No items are indexed under {index}");
        }

        T item = enumerator.Current;
        Remove(item); 
        return item;
    }

    public int GetHighestIndex()
    {
        IEnumerator<int> enumerator = setDictionary.Keys.GetEnumerator();
        int? retval = null;
        while(enumerator.MoveNext())
        {
            if(retval == null || retval < enumerator.Current)
            {
                retval = enumerator.Current;
            }
        }

        if(retval == null)
        {
            throw new SystemException("Indexed set is empty.");
        }

        return (int)retval;
    }
}
