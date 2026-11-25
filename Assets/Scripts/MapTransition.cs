using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    //mapBoundary is a PolygonCollider2D component that defines the boundaries of the map. It is used to restrict the player's movement within the defined area. The player will not be able to move outside of this boundary, ensuring that they stay within the playable area of the map.
    [SerializeField] PolygonCollider2D mapBoundary;
    //confiner is a CinemachineConfiner2D component that is used to confine the camera's movement within the specified map boundaries. It ensures that the camera does not go outside of the defined area, providing a smooth and controlled camera experience for the player.
    CinemachineConfiner2D confiner;
    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPosition;
    [SerializeField] float additivePos =1.5f;
    enum Direction{up, down, left, right, teleport};
    private void Awake()
    {
        confiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }
    //this function is called when the player enters the trigger area of the map transition. It updates the camera's confiner to the new map boundary, ensuring that the camera stays within the defined area of the new map. This allows for smooth transitions between different areas of the game world while keeping the camera focused on the player.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.gameObject);
 
            MapController_Manual.Instance?.HightlightArea(mapBoundary.name);
            MapController_Dynamic.Instance?.UpdateCurrentArea(mapBoundary.name);
        }
    }
    async void FadeTransition(GameObject player)
    {
            PauseController.SetPause(true);
            await ScreenFader.Instance.FadeOut();
             //m_BoundingShape2D is a property of the CinemachineConfiner2D component that defines the shape of the area within which the camera is confined. By setting this property to the new map boundary, we ensure that the camera will only move within the specified area, providing a consistent and controlled camera experience for the player.
            confiner.BoundingShape2D  = mapBoundary;
            UpdatePlayerPosition(player);

            await ScreenFader.Instance.FadeIn(); 
            PauseController.SetPause(false);
    }
    private void UpdatePlayerPosition(GameObject player)
    { 
        if(direction == Direction.teleport)
        {
            player.transform.position = teleportTargetPosition.position;

            return;
        }
        Vector3 newPosition = player.transform.position;
        switch (direction)
        {
            case Direction.up:
                newPosition.y += additivePos;
                break;
            case Direction.down:
                newPosition.y -= additivePos;
                break;
            case Direction.left:
                newPosition.x -= additivePos;
                break;
            case Direction.right:
                newPosition.x += additivePos;
                break;
        }
        player.transform.position = newPosition;
    }
}
