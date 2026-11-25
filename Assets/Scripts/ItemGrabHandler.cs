 using UnityEngine;
 using UnityEngine.EventSystems;

public class ItemGrabHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler
{ 
    Transform originalParent; // Store the original parent of the item being dragged
    CanvasGroup canvasGroup; // Reference to the CanvasGroup component for controlling item visibility and interaction
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float minDropDistance = 2f; // Minimum distance from the player to drop the item
    public float maxDropDistance = 3f; // Maximum distance from the player to drop the item
    private InventoryController inventoryController;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>(); // Get the CanvasGroup component attached to the item 
        inventoryController = InventoryController.Instance;
    }
    public void OnBeginDrag( PointerEventData eventData)
    {
        //transform is used to access the item's Transform component, which represents its position, rotation, and scale in the scene. The originalParent variable is assigned the current parent of the item being dragged, allowing it to be restored later if needed. The SetParent method is called on the item's Transform to change its parent to the root of the scene (transform.root), enabling it to be dragged freely without being constrained by its original parent hierarchy. The blocksRaycasts property of the CanvasGroup component is set to false, allowing other UI elements to receive raycast events while the item is being dragged. Finally, the alpha property of the CanvasGroup is set to 0.6f, making the item partially transparent to indicate that it is being dragged.
        originalParent = transform.parent; // Store the original parent of the item being dragged
        transform.SetParent(transform.root); // Set the item's parent to the root of the scene to allow it to be dragged freely
        canvasGroup.blocksRaycasts = false; // Disable raycast blocking to allow interaction with other UI elements while dragging
        canvasGroup.alpha = 0.6f; // Set the item's transparency to indicate it is being dragged
    } 
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Update the item's position to follow the mouse cursor during dragging
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Re-enable raycast blocking to allow interaction with the item again
        canvasGroup.alpha = 1f; // Reset the item's transparency to fully opaque

        Slot dropSlot = eventData.pointerEnter?.GetComponentInParent<Slot>(); // Get the Slot component of the UI element the item was dropped on, if any
        
        if(dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if(dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>(); // Get the Slot component of the item's original parent slot

        if (dropSlot == originalSlot)
        {
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return;
        }

        if (dropSlot != null) // Check if the item was dropped on a valid slot that is empty
        {
            if (dropSlot.currentItem != null) //swap the items if the drop slot is not empty
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if(draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }
                else
                {
                dropSlot.currentItem.transform.SetParent(originalSlot.transform ); // Move the item in the drop slot back to the original parent
                originalSlot.currentItem = dropSlot.currentItem; // Store the current item in the drop slot
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset the position of the item in the drop slot to center it in the original slot
               
                transform.SetParent(dropSlot.transform); // Set the item's parent back to its original parent 
                dropSlot.currentItem = gameObject; // Set the current item reference in the drop slot to the item being dragged               
               
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset the item's position to center it in its parent slot

                }
            }
            else
            {
                originalSlot.currentItem = null; // If the item was not dropped on a valid slot, clear the reference to the current item in the original slot
                transform.SetParent(dropSlot.transform); // Set the item's parent back to its original parent 
                dropSlot.currentItem = gameObject; // Set the current item reference in the drop slot to the item being dragged               
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset the item's position to center it in its parent slot
            } 
        }
        else
        {
            //if where were dropping is not within inventory
            if(!IsWithinInventory(eventData.position))
            {
                //drop our item
                DropItem(originalSlot); // Call the DropItem method to drop the item in the world
            }
            else
            {
             transform.SetParent(originalParent); // If the item was not dropped on a valid slot, set its parent back to the original parent
             GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Reset the item's position to center it in its parent slot
            }
        }
     }
     bool IsWithinInventory(Vector2 mousePosition)
     {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>(); // Get the RectTransform of the inventory panel
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition); // Check if the mouse position is within the bounds of the inventory panel
     } 
     void DropItem(Slot originalSlot)
    {
        Item item = GetComponent<Item>();
        int quantity = item.quantity;
        if (quantity > 1)
        {
            item.RemoveFromStack();

            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            quantity = 1;
        }
        else
        {
            originalSlot.currentItem = null;
        }
        //Find player and drop item in the world
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Find the player
        if (playerTransform == null)
        {
            Debug.LogError("Player not found in the scene."); // Log an error if the player is not found
            return;
        }
        //Random drop position around the player
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance); // Generate a random drop position around the player within the specified distance range
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset; // Calculate the drop position

        //Instantiate the item in the world at the drop position
        GameObject dropItem = Instantiate(gameObject, dropPosition, Quaternion.identity); // Instantiate the item in the world at the calculated drop position
        Item droppedItem = dropItem.GetComponent<Item>();
        droppedItem.quantity = 1;
        dropItem.GetComponent<BounceEffect>().StartBounce();

        //destroy the UI 
        if(quantity<=1&&originalSlot.currentItem == null)
        {
            Destroy(gameObject); // Destroy the item in the inventory (UI) after dropping it in the world
        }
   }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            SplitStack();
        }
    }
    private void SplitStack()
    {
        Item item = GetComponent<Item>();
        if(item == null || item.quantity <= 1)return;
        
        int splitAmount = item.quantity/2;
        if(splitAmount <= 0) return;

        item.RemoveFromStack(splitAmount);
        GameObject newItem = item.CloneItem(splitAmount); 

        if(inventoryController == null || newItem == null) return;
        foreach(Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot != null && slot.currentItem == null)
            {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }
        item.AddToStack(splitAmount);
        Destroy(newItem);
    }
}
