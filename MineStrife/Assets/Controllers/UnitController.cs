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
using Assets.Units;

namespace Assets.Controllers
{
    public class UnitController : MonoBehaviour
	{
        public static UnitController Instance { get; protected set; }

        void Start()
        {
            if (Instance != null)
            { 
                Debug.LogError(this.GetType().ToString() + " already instanced");
            }
            Instance = this;
        }

        public void CreateUnitAt(Vector2 inPos, UnitType inUnitType)
        {
            var spawnPos = GetClosestFreePosition(inPos);
            //if the player can afford the building
            if (spawnPos != null)
            {
                var prefab = UnitPrefabController.Instance.GetPrefab(inUnitType);

                //instance a new build prefab and place it on the screen
                GameObject unit_go = Instantiate(prefab);
                unit_go.transform.SetParent(this.transform, true);
                unit_go.name = "Unit";

                //create a new unit
                new Unit(new UnityObjectWrapper(unit_go), spawnPos.Value, inUnitType);
            }         
        }

        public bool IsPositionFree(Vector2 inPosition)
        {
            var allGameObjects = World.all_worldEntity.AsReadOnly().Select(x => x.ViewObject.GetUnityGameObject()).ToList();
            //Debug.Log("Search pos (" + inPosition.x + " : " + inPosition.y + ")");

            foreach (var obj in allGameObjects)
            {
                var box = obj.GetComponent<BoxCollider2D>();
                if(box != null)
                {
                    if(box.OverlapPoint(inPosition))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Vector2? GetClosestFreePosition(Vector2 inStartPosition)
        {
            //starting at the buildings position, 
            int range = 0;// (int)Math.Min(inBuildingWidth.x, inBuildingWidth.y) - 1;
            Vector2? freePosition = null;
            while (freePosition == null && range < 10)
            {
                //search from bottom left to bottom right
                for (int x = (int)(inStartPosition.x - range); x < (inStartPosition.x + range) && (freePosition == null); x++)
                {
                    var checkPos = new Vector2(x, (int)(inStartPosition.y - range));
                    if (IsPositionFree(checkPos))
                    {
                        freePosition = checkPos;
                    }
                }

                //search from bottom right to top right
                for (int y = (int)(inStartPosition.y - range); y < (inStartPosition.y + range) && (freePosition == null); y++)
                {
                    var checkPos = new Vector2((int)(inStartPosition.x + range), y);
                    if (IsPositionFree(checkPos))
                    {
                        freePosition = checkPos;
                    }
                }

                //search from top right to top left
                for (int x = (int)(inStartPosition.x + range); x > (inStartPosition.x - range) && (freePosition == null); x--)
                {
                    var checkPos = new Vector2(x, (int)(inStartPosition.y + range));
                    if (IsPositionFree(checkPos))
                    {
                        freePosition = checkPos;
                    }
                }

                //search from top left to bottom right
                for (int y = (int)(inStartPosition.y + range); y > (inStartPosition.y - range) && (freePosition == null); y--)
                {
                    var checkPos = new Vector2((int)(inStartPosition.x - range), y);
                    if (IsPositionFree(checkPos))
                    {
                        freePosition = checkPos;
                    }
                }

                range++;
            }

            return freePosition;
        }
	}
}
