using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController_Dynamic : MonoBehaviour
{
    [Header("UI references")]
    public RectTransform mapParent;
    public GameObject areaPrefab;
    public RectTransform playerIcon;

    [Header("Colours")]
    public Color defaultColour = Color.gray;
    public Color currentAreaColor = Color.green;

    [Header("Map Settings")]
    public GameObject MapBoundary;
    public PolygonCollider2D initialArea; //Initial starting Area
    public float mapScale = 10f;//Adjust map size on UI
    private PolygonCollider2D[] mapAreas;

    private Dictionary<string, RectTransform> uiAreas = new Dictionary<string, RectTransform>();

    public static MapController_Dynamic Instance {get;set;}
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        mapAreas = MapBoundary.GetComponentsInChildren<PolygonCollider2D>();
    }
    //Generate Map
    public void GenerateMap(PolygonCollider2D newCurrentArea = null)
    {
        PolygonCollider2D currentArea = newCurrentArea != null ? newCurrentArea : initialArea;
        ClearMap();

        foreach(PolygonCollider2D area in mapAreas)
        {
            //create area ui
            CreateAreaUI(area,area == currentArea);
        }
        //moveplayer icon
        MovePlayerIcon(currentArea.name);
    }
    //ClearMap
    private void ClearMap()
    {
        foreach(Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }
        uiAreas.Clear(); 
    }
    private void CreateAreaUI(PolygonCollider2D area, bool isCurrent)
    {
        //Instantiate prefab for image
        GameObject areaImage = Instantiate(areaPrefab,mapParent);
        RectTransform rectTransform = areaImage.GetComponent<RectTransform>();
        //get bounds
        Bounds bounds = area.bounds;
        //scale ui image fir map and bounds
        rectTransform.sizeDelta = new Vector2(bounds.size.x *mapScale, bounds.size.y *mapScale);
        rectTransform.anchoredPosition = bounds.center *mapScale;
        //Set colour based on current or not
        areaImage.GetComponent<Image>().color = isCurrent ? currentAreaColor : defaultColour;
        //Add to dictionary
        uiAreas[area.name] = rectTransform;
    }
    //UpdateCurrentArea 
    public void UpdateCurrentArea(string newCurrentArea)
    {
        foreach(KeyValuePair<string, RectTransform> area in uiAreas)
        {
            area.Value.GetComponent<Image>().color = area.Key == newCurrentArea ? currentAreaColor : defaultColour;
        }
        MovePlayerIcon(newCurrentArea);
    }
    //Moveplayer icon
    private void MovePlayerIcon(string newCurrentArea)
    {
        if(uiAreas.TryGetValue(newCurrentArea,out RectTransform areaUI))
        {
            playerIcon.anchoredPosition = areaUI.anchoredPosition;
        }
    }
}

