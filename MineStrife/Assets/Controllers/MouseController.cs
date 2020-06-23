using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Models;
using System;
using Assets.Models.Buildings;
using Assets.Units;
using Assets.Scripts;
using System.Linq;
using Assets.Models.AI;

namespace Assets.Controllers
{
    public class MouseController : MonoBehaviour
    {
        Vector3 lastFramePosition;
        Vector3 startSelectionBox;
        Vector3 MousePosInWorld;
        Vector3 MousePosOnGrid;

        int MaxCameraZoom = 30;
        int MinCameraZoom = 3;

        bool selecting;
        bool placing;
        BuildingDefinition buildingSelected;

        // Use this for initialization
        void Start()
        {
            buildingSelected = null;
            Camera.main.orthographicSize = 10;
        }

        private void UpdateDebugTextWithMousePosition(Vector3 inMousePos)
        {
            var gridPos = GridHelper.IsometricToGrid(VectorHelper.ToVector2(inMousePos));
            var isoPos = GridHelper.GridToIsometric(gridPos);
            var display = "Mouse position iso: " + VectorHelper.Vector3ToString(inMousePos) + "\n" +
                 "Grid position: " + gridPos + "\n" +
                 "ismoetric pos: " + isoPos;

            DisplayController.Instance.DebugInfoText.text = display;
        }

        // Update is called once per frame
        void Update()
        {
            MousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UpdateDebugTextWithMousePosition(MousePosInWorld);

            MousePosOnGrid = GridHelper.IsometricToGrid(VectorHelper.ToVector2(MousePosInWorld));
            MousePosOnGrid.z = 0;

            //if press 'k' key update icon panel
            if(Input.GetKeyDown(KeyCode.K))
            {
                IconPanelController.Instance.QueueShuffelIcons();
            }
            

            //if holding the building key update the ghost sprite
            if (placing)
            {
                ClearBandBox();
                BuildingController.Instance.updateGhostPosition(MousePositionToGridPosition(MousePosOnGrid), buildingSelected);
                //if clicking place a building
                if (Input.GetMouseButtonUp(0))
                {
                    var buildingDefinition = BuildingDefinition.GetBuildingDefinitionForType(buildingSelected.BuildingType);
                    var buildingSize = buildingDefinition.Size;

                    var gridPosition = MousePositionToGridPosition(MousePosOnGrid);
                    var buildingPosition = BuildingController.GetBuildingPositionFromMousePosition(gridPosition, buildingSize);
                    BuildingController.Instance.CreateBuildingAt(buildingPosition, buildingSelected);
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        CancelPlacement();
                    }
                }

                //cancel placement mode if appropriate
                if (Input.GetMouseButtonUp(1) && !Input.GetKey(KeyCode.LeftShift))//clear the ghost as long as shift isnt pressed
                {
                    CancelPlacement();
                }
            }
            
            
            //update selection band box
            if (Input.GetMouseButtonDown(0) && !placing)
            {
                selecting = true;
                startSelectionBox = MousePosInWorld;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ClearBandBox();
                var selection = GetSelection(startSelectionBox, MousePosInWorld);
                if (selection.Count > 0)
                {
                    Select(selection);
                }
                else
                {
                    //check if the mouse was released over a unit or icon
                    BuildingDefinition outBld;
                    if (IconPanelController.Instance.CheckForClick(MousePosInWorld, out outBld))
                    {
                        placing = true;
                        buildingSelected = outBld;
                        DisplayController.Instance.BuildingCostText.text = buildingSelected.BuildingType.ToString() + " " + buildingSelected.BuildingCost.ToString();
                    }
                    else
                    {
                        //check if a unit of building was clicked
                        var clickedEntity = WorldController.Instance.World.EntityAtPosition(MousePosInWorld);
                        if (clickedEntity != null)
                        {
                            Select(clickedEntity);
                        }
                        else
                        {
                            ClearSelection();
                        }
                    }
                }
            }

            //draw selection band box
            if (selecting)
            {
                DrawBandBox();   
            }

            //if right click release
            if(Input.GetMouseButtonUp(1))
            {
                //if units selected
                if(WorldController.Instance.CurrentSelection.Count > 0) //TODO check if at least one is a unit
                {
                    GiveOrderToUnits(MousePosInWorld);
                } 
            }

            //allow the user to drag the screen
            DragScreen(MousePosInWorld);
            UpdateZoomLevel();
        }

        public void GiveOrderToUnits(Vector3 MousePosInWorld)
        {
            //formations are just causing issues with pathfinding
            //var positions = FormationCalculator.findPositionsForFormation(
            //    WorldController.Instance.CurrentSelection.Select(x => x.Position).ToList(),
            //    new Vector2(currentMousePosOnFloor.x, currentMousePosOnFloor.y),
            //    0.8f
            //    );


            CommandTypes commandT = CommandTypes.MOVE;
            WorldEntity clickedEntity = null;

            Debug.Log("checking for entity at position (" + MousePosInWorld.x + "," + MousePosInWorld.y + ")");

            var entityAtPosition = World.Instance.EntityAtPosition(MousePosInWorld);
            
            if (entityAtPosition != null)
            {
                Debug.Log("entity at position");
            }

            //check if entity is a unit
            if (entityAtPosition != null && 
                entityAtPosition.GetType() == typeof(Unit))
            {
                Debug.Log("clicker unit");
                Unit entityAsUnit = (Unit)entityAtPosition;
                clickedEntity = entityAsUnit;

                if (entityAsUnit.getTeam() == WorldController.PlayerTeam)
                {
                    Debug.Log("player unit");
                    //unit on same team
                    commandT = CommandTypes.FOLLOW;
                }
                else if (entityAsUnit.getTeam() == WorldController.nutralTeam)
                {
                    Debug.Log("neutral unit");
                    //just move up to neutral units
                    //TODO force attack functionality?
                    commandT = CommandTypes.MOVE;
                }
                else
                {
                    Debug.Log("enemy unit");
                    //assume enemy unit = attack
                    commandT = CommandTypes.ATTACK;
                }
            }

            //check if entity is a building
            ProductionBuilding entityAsBuilding = null;
            if (entityAtPosition != null &&
                entityAtPosition.GetType() == typeof(ProductionBuilding))
            {
                Debug.Log("clicked building");
                entityAsBuilding = (ProductionBuilding)entityAtPosition;
                clickedEntity = entityAsBuilding;

                if (entityAsBuilding.getTeam() == WorldController.PlayerTeam)
                {
                    Debug.Log("Player building");
                    //unit on same team
                    if(entityAsBuilding.IsUnderConstruction)
                    {
                        //TODO 
                        Debug.Log("under construction building");
                        commandT = CommandTypes.BUILD;
                    }
                    else
                    {
                        //TODO logic to figure out if unit should work there, or move there, or build etc
                        //based on if the building needs workers and if the unit can work there
                        Debug.Log("other typoe of building");
                        commandT = CommandTypes.MOVE;
                    }
                    
                }
                else if (entityAsBuilding.getTeam() == WorldController.nutralTeam)
                {
                    Debug.Log("neutral building");
                    //just move up to neutral units
                    //TODO force attack functionality?
                    commandT = CommandTypes.MOVE;
                }
                else
                {
                    Debug.Log("enemy building");
                    //assume enemy unit = attack
                    commandT = CommandTypes.ATTACK;
                }
            }

            //give command to units
            for (int i = 0; i < WorldController.Instance.CurrentSelection.Count; i++)
            {
                if (WorldController.Instance.CurrentSelection[i].GetType() == typeof(Unit))
                {
                    //var closestPosition = positions.OrderBy(x => VectorHelper.getDistanceBetween(x, WorldController.Instance.CurrentSelection[i].Position)).First();
                    //positions.Remove(closestPosition);
                    var unit = (Unit)WorldController.Instance.CurrentSelection[i];
                    if (unit.Ai.SupportsBehaviour(commandT))
                    {
                        Debug.Log("Supports behaviour " + commandT.ToString());
                        unit.Ai.AddCommand(
                        new Command(commandT, VectorHelper.ToVector2(MousePosOnGrid), clickedEntity, false),
                        Input.GetKey(KeyCode.LeftShift)
                        );
                    }
                    else
                    {
                        //TODO default to move?
                        unit.Ai.AddCommand(
                        new Command(CommandTypes.MOVE, VectorHelper.ToVector2(MousePosOnGrid), clickedEntity, false),
                        Input.GetKey(KeyCode.LeftShift)
                        );
                    }
                }
            }
        }

        public List<WorldEntity> GetSelection(Vector3 inA, Vector3 inB)
        {
            var size = new Vector3(
                (Math.Max(inA.x, inB.x) - Math.Min(inA.x, inB.x)),
                (Math.Max(inA.y, inB.y) - Math.Min(inA.y, inB.y)));
            var center = new Vector3(
                (Math.Min(inA.x, inB.x) + (size.x / 2.0f)),
                (Math.Min(inA.y, inB.y) + (size.y / 2.0f))
                );

            var selected = new List<WorldEntity>();
            var selectionBox = new Bounds(center, size);

            foreach(var obj in World.all_worldEntity)
            {
                if(obj.GetType() == typeof(Unit))
                {
                    var objBounds = obj.ViewObject.GetUnityGameObject().GetComponent<BoxCollider2D>();
                    if (objBounds != null && objBounds.bounds.Intersects(selectionBox))
                    {
                        selected.Add(obj);
                    }
                }
            }

            return selected;
        }

        public void Select(WorldEntity inWorldEntity)
        {
            WorldController.Instance.CurrentSelection.Clear();
            WorldController.Instance.CurrentSelection.Add(inWorldEntity);
        }
        public void Select(List<WorldEntity> inWorldEntitys)
        {
            WorldController.Instance.CurrentSelection.Clear();
            WorldController.Instance.CurrentSelection.AddRange(inWorldEntitys);
        }
        public void ClearSelection()
        {
            WorldController.Instance.CurrentSelection.Clear();
        }

        private void CancelPlacement()
        {
            placing = false;
            buildingSelected = null;
            BuildingController.Instance.hideGhost();
            DisplayController.Instance.BuildingCostText.text = "";
        }

        public static Vector2 MousePositionToGridPosition(Vector3 inPosition)
        {
            Vector2 newPos = new Vector3((int)(inPosition.x), (int)(inPosition.y));
            return newPos;
        }

        private void DrawBandBox()
        {
            //only draw if the selection box will be bigger than a single tile
            var distance = startSelectionBox - MousePosInWorld;
             
            //grab the line renderer 
            var lineRender = GetComponent<LineRenderer>();
            //lineRender.SetColors(Color.red, Color.blue);
            var points = new List<Vector3>();
            if (distance.magnitude > 1)
            {
                points.Add(new Vector3(startSelectionBox.x, startSelectionBox.y, -1));
                points.Add(new Vector3(MousePosInWorld.x, startSelectionBox.y, -1));
                points.Add(new Vector3(MousePosInWorld.x, MousePosInWorld.y, -1));
                points.Add(new Vector3(startSelectionBox.x, MousePosInWorld.y, -1));
                points.Add(new Vector3(startSelectionBox.x, startSelectionBox.y, -1));

                points.Add(new Vector3(startSelectionBox.x, startSelectionBox.y, -1));
                points.Add(new Vector3(startSelectionBox.x, MousePosInWorld.y, -1));
                points.Add(new Vector3(MousePosInWorld.x, MousePosInWorld.y, -1));
                points.Add(new Vector3(MousePosInWorld.x, startSelectionBox.y, -1));
                points.Add(new Vector3(startSelectionBox.x, startSelectionBox.y, -1));
            }

            lineRender.SetVertexCount(points.Count);
            lineRender.SetWidth(0.1f, 0.1f);
            lineRender.SetPositions(points.ToArray());
        }

        private void ClearBandBox()
        {
            selecting = false;
            var lineRender = GetComponent<LineRenderer>();
            var points = new List<Vector3>();
            lineRender.SetPositions(points.ToArray());
            lineRender.SetVertexCount(points.Count);
        }

        private void UpdateZoomLevel()
        {
            Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinCameraZoom, MaxCameraZoom);
        }

        private void DragScreen(Vector3 currentPosition)
        {
            if (Input.GetMouseButton(2))
            {
                Vector3 diff = lastFramePosition - currentPosition;
                Camera.main.transform.Translate(diff);
            }
            lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private Tile GetTileAtWorldPosition(int inX, int inY)
        {
            return WorldController.Instance.World.GetTileAt(inX, inY);
        }
    }

}
