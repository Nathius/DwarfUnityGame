using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Models.Display;
using Assets.Models.Buildings;
using System.Linq;

public class IconPanelController : MonoBehaviour {
    List<Icon> CurrentIcons;

    public static IconPanelController Instance { get; protected set; }

	// Use this for initialization
	void Start () {
        if (Instance != null)
        {
            Debug.LogError("Display controller already instanced");
        }
        Instance = this;

        CurrentIcons = new List<Icon>();
	}

    public void AddIcon(Icon inIcon, GameObject inPrefab)
    {
        var instance = Instantiate(inPrefab);
        instance.transform.parent = this.transform;
        instance.transform.localScale = new Vector3(1.6f, 1.6f);
        inIcon.gameObject = instance;
        CurrentIcons.Add(inIcon);
        ShuffelIcons();
    }

    public void ShuffelIcons()
    {
        for(int i = 0; i < CurrentIcons.Count; i++)
        {
            var thisPos = this.transform.position;
            var go = CurrentIcons[i].gameObject;
            var scale = Camera.main.orthographicSize * 0.18f;
            go.transform.position = new Vector3(thisPos.x + (i * 1.2f * scale), thisPos.y, 0);
        }
    }

    public void RemoveIconForBuildingType(BuildingType inBuildingType)
    {
        var oldIcon = CurrentIcons.Where(x => x.buildingType == inBuildingType).FirstOrDefault();
        if (oldIcon != null)
        {
            CurrentIcons.Remove(oldIcon);
            Destroy(oldIcon.gameObject);
        }
        ShuffelIcons();
    }

    //checks is any icon has been clicked
    public bool CheckForClick(Vector3 InMousePosition, out BuildingDefinition outBuildingDefinition)
    {
        foreach(var icon in CurrentIcons)
        {
            var bounds = icon.gameObject.GetComponent<BoxCollider2D>();
            if (bounds.OverlapPoint(new Vector2(InMousePosition.x, InMousePosition.y)))
            {
                outBuildingDefinition = BuildingDefinition.GetBuildingDefinitionForType(icon.buildingType);
                return true;
            }
        }
        
        outBuildingDefinition = null;
        return false;
    }

	// Update is called once per frame
	void Update () 
    {
        //constantly reset each icons z position to 0
        foreach (var icon in CurrentIcons)
        {
            icon.gameObject.transform.position = new Vector3(icon.gameObject.transform.position.x, icon.gameObject.transform.position.y, 0);
        }
	}
}
