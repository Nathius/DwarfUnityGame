using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Models.Display;
using Assets.Models.Buildings;
using System.Linq;

public class IconPanelController : MonoBehaviour {
    List<Icon> CurrentIcons;

    public static IconPanelController Instance { get; protected set; }

    private bool NeedsIconShuffel = false;

	// Use this for initialization
	void Start () {
        if (Instance != null)
        {
            Debug.LogError(this.GetType().ToString() + " already instanced");
        }
        Instance = this;

        CurrentIcons = new List<Icon>();
        NeedsIconShuffel = true;
        Debug.Log("need shuffel from start");
	}

    public void AddIcon(Icon inIcon, GameObject inPrefab)
    {
        var instance = Instantiate(inPrefab);
        instance.transform.parent = Instance.transform;
        instance.transform.localScale = new Vector3(1.6f, 1.6f);
        inIcon.gameObject = instance;
        CurrentIcons.Add(inIcon);
        //ShuffelIcons();
        NeedsIconShuffel = true;
        Debug.Log("need shuffel from adding icons");
    }

    public void QueueShuffelIcons()
    {
        NeedsIconShuffel = true;
    }

    public void ShuffelIcons()
    {
        var panelWidth = Instance.GetComponent<RectTransform>().rect.width;
        Debug.Log("shuffeling icons with pos (" + Instance.transform.position.x + "," + Instance.transform.position.y + "), panel width: " + panelWidth + ", size " + (Camera.main.orthographicSize));

        //icon panel position, get x of left border by subtracting half of width
        var startX = Instance.transform.position.x - (panelWidth * 0.5f);

        for(int i = 0; i < CurrentIcons.Count; i++)
        {
            //each icon object
            var go = CurrentIcons[i].gameObject;

            var width = go.GetComponent<SpriteRenderer>().bounds.extents.x * 2;

            // offset each icon allong the icon bar
            go.transform.position = new Vector3(startX + (i * width), Instance.transform.position.y, 0);
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
        NeedsIconShuffel = true;
        Debug.Log("need shuffel from removing icons");
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

        //only shuffel during an update
        if (NeedsIconShuffel)
        {
            ShuffelIcons();
            NeedsIconShuffel = false;
            Instance.transform.hasChanged = false;
        }
	}
}
