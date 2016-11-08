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
using Assets.Units;

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
        private List<GameObject> Hilights { get; set; }

        private bool HaveInitIconPanel;

        // Use this for initialization
        void Start()
        {
            if (Instance != null)
            {
                Debug.LogError(this.GetType().ToString() + " already instanced");
            }
            Instance = this;
            Hilights = new List<GameObject>();
        }

        private void UpdateSelectionPanel()
        {
            if (WorldController.Instance.CurrentSelection.Count > 1)
            {
                InfoPanelText.text = "Selected " + WorldController.Instance.CurrentSelection.Count + " entities";
            }
            else if (WorldController.Instance.CurrentSelection.Count == 1)
            {
                InfoPanelText.text = "Selected " + WorldController.Instance.CurrentSelection.First().ToString();
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
            for (int i = 0; i < (int)RESOURCE_TYPE._COUNT; i++)
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

            UpdateSelectionPanel();

            //very bad for performance, should keep all hilights actuve and move them, 
            //only creating or deleting when required
            //could also put hilight objects in a pool
            ClearHilights();
            if (WorldController.Instance.CurrentSelection.Count > 0)
            {
                foreach (var obj in WorldController.Instance.CurrentSelection)
                {
                    var hilight = Instantiate(SelectionHilightPrefab);
                    hilight.transform.position = obj.ViewObject.GetSpriteCenter().Value;

                    //scale the hilight ring to the size of the unit
                    var spriteSize = obj.ViewObject.GetSpriteSize().Value * 0.5f;
                    hilight.transform.localScale = new Vector3(spriteSize.x, spriteSize.y);

                    Hilights.Add(hilight);
                }
            }
        }

        public void ClearHilights()
        {
            while (Hilights.Count > 0)
            {
                var obj = Hilights.First();
                Hilights.RemoveAt(0);
                Destroy(obj.gameObject);
                Destroy(obj);

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
