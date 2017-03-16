using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDict<T> : Dictionary<int, List<T>> {

    public bool AddEntry(int key, T value)
    {
        bool result = false;

        if (value != null)
        {
            bool containsKey = ContainsKey(key);
            bool notHasValue = false;

            if (containsKey)
                notHasValue = !Has(key, value);

            //  make sure the key is not associated with this value
            if (containsKey && notHasValue)
            {
                this[key].Add(value);
                result = true;
            }
            else if (notHasValue)
            {
                Add(key, new List<T>());
                this[key].Add(value);

                result = true;
            }
        }     

        return result;
    }

    public bool RemoveValueFromEntry(int key, T value)
    {
        bool result = false;

        if (ContainsKey(key) && this[key].Contains(value))
        {
            this[key].Remove(value);
            result = true;
        }  
        
        return result;
    }

    // removes every entry this value shows up in
    public void RemoveAllOfValue(T value)
    {
        foreach(int key in Keys)
        {
            if (this[key].Contains(value))
                this[key].Remove(value);
        }
    }

    // removes every key value association with this key
    public bool RemoveKey(int key)
    {
        bool result = false;

        if (ContainsKey(key))
        {
            Remove(key);
            result = true;
        }  
        
        return result;
    }

    // may return null
    public ICollection<T> GetValuesAsCollection(int key)
    {
        return ContainsKey(key) ? this[key] : null;
    }

    // may return null
    public ICollection<int> GetAssociatedKeys(T value)
    {
        List<int> keys = new List<int>();

        foreach(int key in Keys)
        {
            if (this[key].Contains(value))
                keys.Add(key);
        }

        return keys;
    }

    // check that the current list does not contain this value
	public bool Has(int key, T entry)
    {
        return this[key].Contains(entry);
    }
}
