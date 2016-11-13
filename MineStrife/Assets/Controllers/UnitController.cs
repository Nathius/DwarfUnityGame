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
using Assets.Models.AI;
using Assets.Scripts;

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
                var definition = UnitDefinition.GetDefinitionForType(inUnitType);

                //instance a new build prefab and place it on the screen
                GameObject unit_go = Instantiate(prefab);
                unit_go.transform.SetParent(this.transform, true);
                unit_go.name = "Unit_" + inUnitType.ToString();

                //create a new unit
                var ai = new AI(definition.Behaviours);
                new Unit(new UnityObjectWrapper(unit_go), spawnPos.Value, inUnitType, ai);
            }         
        }

        public Vector2? GetTilePositionIfFree(Vector2 inPosition)
        {
            var tileCenter = GridHelper.PositionToTileCenter(inPosition);
            var allGameObjects = World.all_worldEntity.AsReadOnly().Select(x => x.ViewObject.GetUnityGameObject()).ToList();

            foreach (var obj in allGameObjects)
            {
                var box = obj.GetComponent<BoxCollider2D>();
                if(box != null)
                {
                    if (box.OverlapPoint(tileCenter))
                    {
                        return null;
                    }
                }
            }
            return tileCenter;
        }

        public Vector2? GetClosestFreePosition(Vector2 inStartPosition)
        {
            //var tileCenterofset = 
            //starting at the buildings position, 
            int range = 0;
            Vector2? freePosition = null;
            while ((freePosition == null || freePosition.Value == null) && range < 10)
            {
                //search from bottom left to bottom right
                for (int x = (int)(inStartPosition.x - range); x < (inStartPosition.x + range) && (freePosition == null); x++)
                {
                    var checkPos = new Vector2(x, (int)(inStartPosition.y - range));
                    freePosition = GetTilePositionIfFree(checkPos);
                }

                //search from bottom right to top right
                for (int y = (int)(inStartPosition.y - range); y < (inStartPosition.y + range) && (freePosition == null); y++)
                {
                    var checkPos = new Vector2((int)(inStartPosition.x + range), y);
                    freePosition = GetTilePositionIfFree(checkPos);
                }

                //search from top right to top left
                for (int x = (int)(inStartPosition.x + range); x > (inStartPosition.x - range) && (freePosition == null); x--)
                {
                    var checkPos = new Vector2(x, (int)(inStartPosition.y + range));
                    freePosition = GetTilePositionIfFree(checkPos);
                }

                //search from top left to bottom right
                for (int y = (int)(inStartPosition.y + range); y > (inStartPosition.y - range) && (freePosition == null); y--)
                {
                    var checkPos = new Vector2((int)(inStartPosition.x - range), y);
                    freePosition = GetTilePositionIfFree(checkPos);
                }

                range++;
            }

            return freePosition;
        }
	}
}
