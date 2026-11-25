using UnityEngine;
using System.Collections.Generic;

[System.Serializable]

public class SaveData
{
    public Vector3 playerPosition; // The player's position in the game world
    public string currentScene; // The name of the current scene the player is in

    public List<InventorySaveData> inventorySaveData ; // A list to hold the player's inventory items
    public List<InventorySaveData> hotbarSaveData; // A list to hold the player's hotbar items
    public List<ChestSaveData> chestSaveData;
} 

[System.Serializable]

public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}