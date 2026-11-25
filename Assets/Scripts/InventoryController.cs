using UnityEngine;
using System.Collections.Generic;
public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary; // Reference to the ItemDictionary script
    public GameObject inventoryPanel; // Reference to the inventory panel GameObject
    public GameObject inventorySlotPrefab; // Reference to the inventory slot prefab
    public int SlotCount; 
    public GameObject[] inventorySlots; // Array to hold the inventory slot GameObjects
    public static InventoryController Instance {get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>(); // Find the ItemDictionary script in the scene
        /*
        for (int i = 0; i < SlotCount; i++)
        {
            //instantiate is used to create a new instance of the inventory slot prefab and set its parent to the inventory panel. This allows for dynamic creation of inventory slots based on the specified SlotCount.
            Slot slot = Instantiate(inventorySlotPrefab, inventoryPanel.transform).GetComponent<Slot>();  // Instantiate a new inventory slot from the prefab and set its parent to the inventory panel
            if (i <inventorySlots.Length)
            {
                // Check if the current index is within the bounds of the inventorySlots array 
                GameObject item = Instantiate(inventorySlots [i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Set the position of the item to the center of the slot

                slot.currentItem = item; // Assign the instantiated item to the slot's slotContent reference
            } 
        }*/
    } 
    public bool AddItemToInventory(GameObject itemPrefab)
    {
            Item itemToAdd = itemPrefab.GetComponent<Item>();
            if(itemToAdd == null) return false;
            //check if we have this item type inventory
            foreach(Transform slotTransform in inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if (slot != null && slot.currentItem != null) // Check if the slot is empty
                 {
                    Item slotItem = slot.currentItem.GetComponent<Item>();
                    if(slotItem != null && slotItem.ID == itemToAdd.ID)
                    {
                      slotItem.AddToStack();
                      return true;
                    }
                 }
            }
            //look for empty slot
            foreach(Transform slotTransform in inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if (slot != null && slot.currentItem == null) // Check if the slot is empty
                {
                    GameObject newItem = Instantiate(itemPrefab, slot.transform); // Instantiate the item and set its parent to the empty slot
                    newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Set the position of the item to the center of the slot
                    slot.currentItem = newItem; // Assign the instantiated item to the slot's currentItem reference
                    return true; // Return true to indicate that the item was successfully added to the inventory
                }
            }
        return false; // Return false if the inventory is full and the item could not be added
        
    }
    public List <InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData= new List<InventorySaveData>(); // Create a new list to hold the inventory save data
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData
                { 
                    itemID = item.ID, 
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = item.quantity 
                }); // Add the item's ID and quantity to the inventory save data list         
             }
        }
        return invData; // Return the list of inventory save data
    }
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        foreach(Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject); // Clear the existing inventory slots
        }
        for(int i = 0; i < SlotCount; i++)
        {
            Instantiate(inventorySlotPrefab, inventoryPanel.transform); // Instantiate new inventory slots based on the specified slotCount
        }
        foreach(InventorySaveData data in inventorySaveData)
        {
            if(data.slotIndex < SlotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>(); // Get the slot at the specified index
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID); // Get the item prefab from the item dictionary using the saved item ID
                if(itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform); // Instantiate the item prefab and set its parent to the slot
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Set the position of the item to the center of the slot
                  
                    Item itemComponent = item.GetComponent<Item>();
                    if(itemComponent != null && data.quantity > 1)
                    {
                        itemComponent.quantity = data.quantity;
                        itemComponent.UpdateQuantityDisplay();
                    }

                    slot.currentItem = item; // Assign the instantiated item to the slot's currentItem reference
                }
            }
        }
    }
}
