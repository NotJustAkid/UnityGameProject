using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas; // Reference to the menu panel GameObject
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false); // Hide the menu panel at the start of the game
        }
        PauseController.SetPause(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) // Check if the Tab key is pressed
        {
            if(!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }
            menuCanvas.SetActive(!menuCanvas.activeSelf); // Toggle the visibility of the menu panel
            PauseController.SetPause(menuCanvas.activeSelf);
        }
    }
}
