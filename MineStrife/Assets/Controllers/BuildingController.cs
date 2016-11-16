using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;
using Assets.Models.Buildings;
using Assets.UnityWrappers;
using Assets.Models.Econemy.ResourceNodes;
using Assets.Controllers.PrefabControllers;
using Assets.Scripts;

namespace Assets.Controllers
{
    public class BuildingController : MonoBehaviour
	{
        public static BuildingController Instance { get; protected set; }
        public GameObject buildingGhostPrefab;
        public GameObject buildingGhost;

        void Start()
        {
            if (Instance != null)
            {
                Debug.LogError(this.GetType().ToString() + " already instanced");
            }
            Instance = this;
            buildingGhost = Instantiate(buildingGhostPrefab);
            buildingGhost.transform.position = new Vector3(0, 0, 0);
            hideGhost();
        }

        public void CreateBuildingAt(Vector3 inPos, BuildingDefinition inBuildingType)
        {
            //if the placement is valid
            if (CanPlaceBuildingAt(buildingGhost, inBuildingType))
            {
                //if the player can afford the building
                if (WorldController.Instance.World.Stockpile.CanAfford(inBuildingType.BuildingCost))
                {
                    var prefab = BuildingPrefabController.Instance.GetPrefab(inBuildingType.BuildingType);

                    //instance a new build prefab and place it on the screen
                    GameObject building_go = Instantiate(prefab);
                    building_go.transform.SetParent(this.transform, true);
                    building_go.name = "building";

                    //create a new building data object
                    Vector2 buildingPosition = new Vector2(inPos.x, inPos.y);
                    var newBuilding = new ProductionBuilding(new UnityObjectWrapper(building_go), buildingPosition, inBuildingType.BuildingType, inBuildingType.Conversion);

                    //withdraw the required resources
                    WorldController.Instance.World.Stockpile.RemoveStock(inBuildingType.BuildingCost);

                    if(inBuildingType.BuildingType == BuildingType.CITY_CENTER)
                    {
                        WorldController.Instance.World.CityCenter = (Building)newBuilding;
                        IconPanelController.Instance.RemoveIconForBuildingType(BuildingType.CITY_CENTER);
                    }
                }
            }          
        }

        public void hideGhost()
        {
            buildingGhost.SetActive(false);
            ClearLine();
        }

        public static Vector2 GetBuildingPositionFromMousePosition(Vector2 inPosition, Vector2 inSize)
        {
            var centerOfset = GridHelper.OffsetToBuildingCenter(inPosition, inSize);
            return centerOfset;
        }

        public void updateGhostPosition(Vector2 inMouseOnGridPosition, BuildingDefinition inBuildingDefinition)
        {
            ClearLine();
            if(buildingGhost != null)
            {
                buildingGhost.SetActive(true);

                var buildingPlacementPosition = GetBuildingPositionFromMousePosition(inMouseOnGridPosition, inBuildingDefinition.Size);
                buildingGhost.transform.position = buildingPlacementPosition;

                if (CanPlaceBuildingAt(buildingGhost, inBuildingDefinition))
                {
                    //if no node required
                    if (inBuildingDefinition.Conversion == null || ( ! inBuildingDefinition.Conversion.RequiresNodeResources()))
                    {
                        buildingGhost.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else
                    {
                        List<Vector2> foundNodePositions = null;
                        var buildingReferencePoint = GridHelper.BuildingCenter(buildingPlacementPosition, inBuildingDefinition.Size);
                        var foundAllRequiredNodes = HasRequiredResourceNodes(buildingReferencePoint, inBuildingDefinition, out foundNodePositions);

                        //if node required and available
                        if (foundAllRequiredNodes)
                        {
                            var points = GetLineList(buildingReferencePoint, foundNodePositions);
                            DrawLine(points);
                            buildingGhost.GetComponent<SpriteRenderer>().color = Color.green;                            
                        }
                        else 
                        {
                            buildingGhost.GetComponent<SpriteRenderer>().color = Color.yellow;
                        }
                    }
                }
                else
                {
                    buildingGhost.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }

        public bool HasRequiredResourceNodes(Vector2 inPosition, BuildingDefinition inBuildingDefinition, out List<Vector2> outNodePositions)
        {
            var requiredNodes = inBuildingDefinition.Conversion.NodeRequirements();
            bool foundAllRequiredNodes = true;
            var foundNodes = new List<ResourceNode>();

            //check that we can get the required resources from every node
            foreach (var nodeReq in requiredNodes)
            {
                //get resource node within range
                var nodes = ResourceNode.NodesWithinProximityOfPoint(inPosition, nodeReq.harvestDistance, nodeReq.resourceAmmount.ResourceType);
                if (nodes != null && nodes.Count > 0)
                {
                    foundNodes.AddRange(nodes);
                }
                else
                {
                    foundAllRequiredNodes = false;
                }
            }

            outNodePositions = foundNodes.Select(x => x.ReferencePosition()).ToList();
            return foundAllRequiredNodes;
        }

        public bool CanPlaceBuildingAt(GameObject inBuilding_go, BuildingDefinition inBuildingType)
        {
            bool canPlace = true;
            var ghostColider = inBuilding_go.GetComponent<BoxCollider2D>();

            //check if we have a city center or are trying to build ine
            if (WorldController.Instance.World.CityCenter == null && inBuildingType.BuildingType != BuildingType.CITY_CENTER)
            {
                return false;
            }

            //check that the building is within range of the city center
            if (WorldController.Instance.World.CityCenter != null && inBuildingType.BuildingType != BuildingType.CITY_CENTER)
            {
                int cityLimitRange = 14;
                
                var distance = Vector3.Distance(inBuilding_go.transform.position, WorldController.Instance.World.CityCenter.ViewObject.GetUnityGameObject().transform.position);

                if (distance > cityLimitRange)
                {
                    return false;
                }
            }

            var allGameObjects = World.all_worldEntity.Select(x => x.ViewObject.GetUnityGameObject()).ToList();

            //check if the building can be placed
            for (int i = 0; (i < allGameObjects.Count()) && canPlace; i++)
            {
                var otherBuildingColider = allGameObjects[i].GetComponent<BoxCollider2D>();

                if (otherBuildingColider != null && otherBuildingColider.enabled && ghostColider.bounds.Intersects(otherBuildingColider.bounds))
                {
                    canPlace = false;
                }
            }


            return canPlace;
        }

        private List<Vector3> GetLineList(Vector3 inBuildingPos, List<Vector2> inPoints)
        {
            var lineList = new List<Vector3>();
            foreach(var point in inPoints)
            {
                lineList.Add(VectorHelper.ToVector3(inBuildingPos, -1));
                lineList.Add(VectorHelper.ToVector3(point, -1));
            }
            return lineList;
        }

        private void DrawLine(List<Vector3> inPoints)
        {
            //grab the line renderer 
            var lineRender = GetComponent<LineRenderer>();
            lineRender.sortingLayerName = "Effects";
            //lineRender.SetColors(Color.red, Color.blue);
            var points = inPoints;
            lineRender.SetVertexCount(points.Count);
            lineRender.SetWidth(0.2f, 0.2f);
            lineRender.SetPositions(points.ToArray());
        }

        private void ClearLine()
        {
            var lineRender = GetComponent<LineRenderer>();
            var points = new List<Vector3>();
            lineRender.SetPositions(points.ToArray());
            lineRender.SetVertexCount(points.Count);
        }
	}
}
