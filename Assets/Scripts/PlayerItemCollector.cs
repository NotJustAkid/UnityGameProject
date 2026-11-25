using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>(); // Find the InventoryController script in the scene
    }

    // Update is called once per frame
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item")) // Check if the collided object has the "Item" tag
        {
            Item item = collision.GetComponent<Item>(); // Get the Item component from the collided object
            if (item != null)
            {
                Debug.Log("Adding item to inventory: " + item.Name);
                bool itemAdded = inventoryController.AddItemToInventory(collision.gameObject); // Add the item to the player's inventory using its ID

                if (itemAdded)
                {
                    Debug.Log("PlayerItemCollector: Pickup triggered");
                    item.Pickup();
                    Destroy(collision.gameObject); // Destroy the item GameObject after adding it to the inventory
                }
            }
        }
    }
}
