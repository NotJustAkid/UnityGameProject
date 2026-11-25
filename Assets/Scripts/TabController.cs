using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;  // Array of Image components for each tab
    public GameObject[] tabContents;  // Array of GameObjects for each tab's content    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivateTab(0); // Activate the first tab by default
    }

    // Update is called once per frame
    public void ActivateTab(int tabIndex)
    {
        for (int i = 0; i < tabContents.Length; i++)
        {
            // Check if the current index matches the selected tab index
            if (i == tabIndex)
            {
                tabImages[i].color = Color.white; // Set the active tab's color to white
                tabContents[i].SetActive(true); // Show the active tab's content
            }
            else
            {
                tabImages[i].color = Color.gray; // Set inactive tabs' color to gray
                tabContents[i].SetActive(false); // Hide inactive tabs' content
            }
        }
    }
    
        
    
}
