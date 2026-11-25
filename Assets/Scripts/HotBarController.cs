using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HotBarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject inventorySlotPrefab;
    public int slotCount = 10;

    private ItemDictionary itemDictionary;
    private Key[]hotbarKeys;

    public void Awake()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i<9? (Key)((int)Key.Digit1 + i) : Key.Digit0; // Assuming you want to use numeric keys 1-10
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                UseItemInSlot(i);// Call the method to use the item in the corresponding hotbar slot
            }
        }
    }
    void UseItemInSlot(int slotIndex)
    {
        Slot slot = hotbarPanel.transform.GetChild(slotIndex).GetComponent<Slot>();
        if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                item.UseItem();
            }
        
    }
     public List <InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData= new List<InventorySaveData>(); // Create a new list to hold the inventory save data
        foreach(Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData() { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() }); // Add the item's ID and quantity to the inventory save data list
            }
        }
        return hotbarData; // Return the list of inventory save data
    }
    public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
    {
        foreach(Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject); // Clear the existing inventory slots
        }
        for(int i = 0; i < slotCount; i++)
        {
            Instantiate(inventorySlotPrefab, hotbarPanel.transform); // Instantiate new inventory slots based on the specified slotCount
        }
        foreach(InventorySaveData data in inventorySaveData)
        {
            if(data.slotIndex < slotCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>(); // Get the slot at the specified index
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID); // Get the item prefab from the item dictionary using the saved item ID
                if(itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform); // Instantiate the item prefab and set its parent to the slot
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Set the position of the item to the center of the slot
                    slot.currentItem = item; // Assign the instantiated item to the slot's currentItem reference
                }
            }
        }
    }
}
