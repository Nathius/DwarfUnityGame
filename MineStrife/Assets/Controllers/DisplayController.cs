using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Models.Econemy;
using System.Linq;
using Assets.Controllers.PrefabControllers;
using Assets.Models.Buildings;
using Assets.Models.Display;
using Assets.Models;
using System.Collections.Generic;

namespace Assets.Controllers
{

    public class DisplayController : MonoBehaviour
    {
        public static DisplayController Instance { get; protected set; }
        public Text StockCountDisplay;
        public Text DebugInfoText;
        public Text InfoPanelText;
        public Text BuildingCostText;
        public GameObject ProductionNumberPrefab;
        public GameObject Canvas;
        public GameObject SelectionHilightPrefab;
        private GameObject Hilight { get; set; }

        private bool HaveInitIconPanel;

        // Use this for initialization
        void Start()
        {
            if (Instance != null)
            {
                Debug.LogError(this.GetType().ToString() + " already instanced");
            }
            Instance = this;
        }

        private void UpdateSelectionPanel(WorldEntity inWorldEntity)
        {
            if (inWorldEntity != null)
            {
                InfoPanelText.text = "Selected " + inWorldEntity.ToString();
            }
            else
            {
                InfoPanelText.text = "";
            }
            
        }

        private void InitBuildingIconPannel()
        {
            //for each icon create a prefab and add it to the icon pannel
            var cityCenter = BuildingIconPrefabController.Instance.GetPrefab(BuildingType.CITY_CENTER);
            IconPanelController.Instance.AddIcon(new Icon(BuildingType.CITY_CENTER), cityCenter);

            var farmer = BuildingIconPrefabController.Instance.GetPrefab(BuildingType.FARMER);
            IconPanelController.Instance.AddIcon(new Icon(BuildingType.FARMER), farmer);

            var woodCutter = BuildingIconPrefabController.Instance.GetPrefab(BuildingType.WOOD_CUTTER);
            IconPanelController.Instance.AddIcon(new Icon(BuildingType.WOOD_CUTTER), woodCutter);

            var mill = BuildingIconPrefabController.Instance.GetPrefab(BuildingType.MILL);
            IconPanelController.Instance.AddIcon(new Icon(BuildingType.MILL), mill);

            var barracks = BuildingIconPrefabController.Instance.GetPrefab(BuildingType.BARRACKS);
            IconPanelController.Instance.AddIcon(new Icon(BuildingType.BARRACKS), barracks);
        }

        private void UpdateStockpileReadout()
        {
            string tempText = "";
            for (int i = 0; i < (int)RESOURCE_TYPE.COUNT; i++)
            {
                tempText += (((RESOURCE_TYPE)i).ToString()) + ": " + WorldController.Instance.World.Stockpile.GetStockLevel((RESOURCE_TYPE)i) + "\n";
            }
            StockCountDisplay.text = tempText;
        }

        // Update is called once per frame
        void Update() 
        {
            if (!HaveInitIconPanel && IconPanelController.Instance != null)
            {
                InitBuildingIconPannel();
                HaveInitIconPanel = true;
            }

            UpdateStockpileReadout();

            UpdateSelectionPanel(WorldController.Instance.CurrentSelection);

            if (WorldController.Instance.CurrentSelection != null)
            {
                if (Hilight == null)
                {
                    Hilight = Instantiate(SelectionHilightPrefab);
                }
                Hilight.SetActive(true);
                Hilight.transform.position = WorldController.Instance.CurrentSelection.Position;
            }
            else if(Hilight != null)
            {
                Hilight.SetActive(false);
            }
	    }

        public void CreateProductionNumber(Vector2 inPos, ResourceBundle inBundle)
        {
            //format production display string
            string display = string.Join(", ", inBundle.resources.Select(x => "+ " + x.ToString()).ToArray());
            CreateProductionLable(inPos, display);
        }


        public void CreateProductionLable(Vector2 inPos, string inText)
        {
            GameObject number = Instantiate(ProductionNumberPrefab);
            number.transform.SetParent(Canvas.transform, true);
            number.transform.position = inPos;
            var text = number.GetComponent<Text>();
            text.text = inText;
        }
    }
}
