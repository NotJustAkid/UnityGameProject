using UnityEngine;
using System.IO;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class SaveController : MonoBehaviour
{
    private string saveLocation; // Path to the save file
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private InventoryController inventoryController; // Reference to the InventoryController script
    private HotBarController hotbarController; // Reference to the HotBarController script

    private Chest[] chests;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        InitializeComponent();
        LoadGame(); // Load the game state when the controller starts
    }
    private void InitializeComponent()
    {
        
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json"); // Set the save file path to a JSON file in the persistent data path
        inventoryController = FindAnyObjectByType<InventoryController>(); // Find the InventoryController script in the scene
        hotbarController = FindAnyObjectByType<HotBarController>(); // Find the HotBarController script in the scene
        chests = FindObjectsByType<Chest>();
        
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData()
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position, // Get the player's current position
            currentScene = FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D.name, // Get the name of the current scene from the CinemachineConfiner2D component
            inventorySaveData = inventoryController.GetInventoryItems(), // Get the player's inventory items from the InventoryController
            hotbarSaveData = hotbarController.GetHotbarItems(), // Get the player's hotbar items from the HotBarController
            chestSaveData = GetChestsState()
        };
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData)); // Serialize the save data to JSON and write it to the save file
    }
    private List<ChestSaveData> GetChestsState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();

        foreach(Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
              chestID = chest.ChestID,
              isOpened = chest.IsOpened  
            };
            chestStates.Add(chestSaveData);
        }
        return chestStates;
    }
    public void LoadGame()
    {
        if (File.Exists(saveLocation)) // Check if the save file exists
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation)); // Read the save file and deserialize the JSON data into a SaveData object
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition; // Set the player's position to the saved position
            PolygonCollider2D savedMapBoundary = GameObject.Find(saveData.currentScene).GetComponent<PolygonCollider2D>();
            FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = savedMapBoundary;// Set the camera's confiner to the saved scene's boundary

            inventoryController.SetInventoryItems(saveData.inventorySaveData); // Set the player's inventory items to the saved items           
            hotbarController.SetHotbarItems(saveData.hotbarSaveData); // Set the player's hotbar items to the saved items

            MapController_Manual.Instance?.HightlightArea(saveData.currentScene);
            MapController_Dynamic.Instance?.GenerateMap(savedMapBoundary);
            
            LoadChestStates(saveData.chestSaveData);
        }
        else
        {
            SaveGame(); // If the save file does not exist, create a new save file with the current game state

            inventoryController.SetInventoryItems(new List<InventorySaveData>()); // Set the player's inventory items to the saved items           
            hotbarController.SetHotbarItems(new List<InventorySaveData>()); // Set the player's hotbar items to the saved items

            MapController_Dynamic.Instance?.GenerateMap();
       } 
    }
    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
         foreach(Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);
            if(chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);    
            }
        }
    }
}
