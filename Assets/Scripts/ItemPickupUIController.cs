using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class ItemPickupUiController : MonoBehaviour
{
    public static ItemPickupUiController Instance { get; private set; } // Singleton instance of the ItemPickupUiController
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject popupPrefab;
    public int maxPopups = 5; // Maximum number of popups to display at once
    public float popupDuration = 3f; // Duration for which the popup is displayed
    private readonly Queue<GameObject> activePopups = new();// Queue to keep track of active popups
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance to this object if it is not already set
        }
        else
        {
            Debug.LogWarning("Multiple instances of ItemPickupUiController detected. Destroying duplicate instance."); // Log a warning if another instance already exists
            Destroy(gameObject); // Destroy this object if another instance already exists
        }
    }
    public void ShowItemPickup(string itemName, Sprite itemIcon)
    {
        Debug.Log("ShowItemPickup called: " + itemName);
        GameObject newPopup = Instantiate(popupPrefab, transform); // Instantiate a new popup from the prefab and set its parent to this controller's transform
        newPopup.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = itemName; // Set the text of the popup to the item's name

        Image itemImage = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>(); // Find the Image component of the itemIcon child object in the popup
        if (itemImage)
        {
            itemImage.sprite = itemIcon; // Set the sprite of the itemIcon to the item's icon
        }
        activePopups.Enqueue(newPopup); // Add the new popup to the queue of active popups
        
        if(activePopups.Count > maxPopups)
        {
            Destroy(activePopups.Dequeue()); // Destroy the oldest popup if the number of active popups exceeds the maximum allowed
        }
        StartCoroutine(FadeOutAndDestroy(newPopup)); // Start the coroutine to fade out and destroy the popup after the specified duration
    }
    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration); // Wait for the specified duration before starting the fade-out
        if(popup == null) yield break; // Exit the coroutine if the popup has already been destroyed

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>(); // Get the CanvasGroup component of the popup for fading
        for(float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if(popup == null) yield break; // Exit the coroutine if the popup has already been destroyed
            canvasGroup.alpha = 1f - timePassed; // Gradually decrease the alpha of the popup to create a fade-out effect
            yield return null; // Wait for the next frame
        }
        Destroy(popup); // Destroy the popup after fading out
    }
}
