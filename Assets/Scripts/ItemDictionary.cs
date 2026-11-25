using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
   public List<Item> itemPrefabs;
   private Dictionary<int, GameObject> itemDictionary;

   private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();
        for(int i = 0; i < itemPrefabs.Count; i++)
        {
            if(itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1; // Assign a unique ID to each item prefab based on its index in the list
        }
    }
    foreach(Item item in itemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject; // Add the item prefab to the dictionary with its ID as the key
        }
    }
    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab); // Try to get the item prefab from the dictionary using the provided itemID
        if(prefab == null)
        {
            Debug.LogWarning($"Item with ID {itemID} not found in the dictionary."); // Log a warning if the item prefab is not found
        }
        return prefab;
    }
}
