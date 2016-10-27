using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Models;
using System;
using Assets.Models.Buildings;

namespace Assets.Controllers
{
    public class MouseController : MonoBehaviour
    {
        Vector3 lastFramePosition;
        Vector3 startSelectionBox;
        Vector3 currentMousePosOnFloor;
        
        bool selecting;
        bool placing;
        BuildingDefinition buildingSelected;

        // Use this for initialization
        void Start()
        {
            buildingSelected = null;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosOnFloor = currentPosition;
            currentMousePosOnFloor.z = 0;

            DisplayController.Instance.DebugInfoText.text = currentPosition.ToString();

            //if holding the building key update the ghost sprite
            if (placing)
            {
                ClearBandBox();
                BuildingController.Instance.updateGhostPosition(MousePositionToGridPosition(currentMousePosOnFloor), buildingSelected);
                //if clicking place a building
                if (Input.GetMouseButtonUp(0))
                {
                    BuildingController.Instance.CreateBuildingAt(MousePositionToGridPosition(currentMousePosOnFloor), buildingSelected);
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
                startSelectionBox = currentMousePosOnFloor;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ClearBandBox();
                //check if the mouse was released over a unit or icon
                BuildingDefinition outBld;
                if (IconPanelController.Instance.CheckForClick(currentMousePosOnFloor, out outBld))
                {
                    placing = true;
                    buildingSelected = outBld;
                    DisplayController.Instance.BuildingCostText.text = buildingSelected.BuildingType.ToString() + " " + buildingSelected.BuildingCost.ToString();
                }
                else 
                {
                    //check if a unit of building was clicked
                    var clickedEntity = WorldController.Instance.World.EntityAtPosition(currentMousePosOnFloor);
                    WorldController.Instance.CurrentSelection = clickedEntity;
                }
            }

            //draw selection band box
            if (selecting)
            {
                DrawBandBox();   
            }

            //allow the user to drag the screen
            DragScreen(currentPosition);
            UpdateZoomLevel();
        }

        public void Select(WorldEntity inWorldEntity)
        {
            WorldController.Instance.CurrentSelection = inWorldEntity;
        }

        private void CancelPlacement()
        {
            placing = false;
            buildingSelected = null;
            BuildingController.Instance.hideGhost();
            DisplayController.Instance.BuildingCostText.text = "";
        }

        public static Vector3 MousePositionToGridPosition(Vector3 inPosition)
        {
            Vector3 newPos = new Vector3((float)Math.Round(inPosition.x), (float)Math.Round(inPosition.y), 0);
            return newPos;
        }

        private void DrawBandBox()
        {
            //only draw if the selection box will be bigger than a single tile
             var distance = startSelectionBox - currentMousePosOnFloor;
             
            //grab the line renderer 
            var lineRender = GetComponent<LineRenderer>();
            //lineRender.SetColors(Color.red, Color.blue);
            var points = new List<Vector3>();
            if (distance.magnitude > 1)
            {
                points.Add(new Vector3(startSelectionBox.x, startSelectionBox.y, -1));
                points.Add(new Vector3(currentMousePosOnFloor.x, startSelectionBox.y, -1));
                points.Add(new Vector3(currentMousePosOnFloor.x, currentMousePosOnFloor.y, -1));
                points.Add(new Vector3(startSelectionBox.x, currentMousePosOnFloor.y, -1));
                points.Add(new Vector3(startSelectionBox.x, startSelectionBox.y, -1));

                points.Add(new Vector3(startSelectionBox.x, startSelectionBox.y, -1));
                points.Add(new Vector3(startSelectionBox.x, currentMousePosOnFloor.y, -1));
                points.Add(new Vector3(currentMousePosOnFloor.x, currentMousePosOnFloor.y, -1));
                points.Add(new Vector3(currentMousePosOnFloor.x, startSelectionBox.y, -1));
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
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3, 20);
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
