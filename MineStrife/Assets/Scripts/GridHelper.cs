using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;
using Assets.Models.Econemy;
using Assets.Models.Econemy.ResourceNodes;

namespace Assets.Scripts
{
	public static class GridHelper
	{
        public static int TileSize = 1;

        public static Vector2 SnapBuildingToGridPosition(Vector2 inPosition)
        {
            Vector2 clippedPosition = new Vector2((int)inPosition.x, (int)inPosition.y);
            return clippedPosition;
        }

        public static void AddBuildingToCollisionMap(Vector2 inPosition, Vector2 inSize)
        {
            //count up by width and height
            for (int w = (int)inPosition.x; (w < (int)(inPosition.x + inSize.x)) && (w < World.Instance.GetWidth()); w++)
            {
                for (int h = (int)inPosition.y; (h < (int)(inPosition.y + inSize.y)) && (h < World.Instance.GetHeight()); h++)
                {
                    World.Instance.tiles[w, h].Cost = 0;
                    World.Instance.tiles[w, h].TileType = TileType.BLOCKED;
                }
            }
        }

        public static bool CanPlaceBuilding(Vector2 inPosition, Vector2 inSize)
        {
            //for each grid square the building would occupy
            for (int w = (int)inPosition.x; (w < (int)(inPosition.x + inSize.x)); w++)
            {
                for (int h = (int)inPosition.y; (h < (int)(inPosition.y + inSize.y)); h++)
                {
                    //check if index outside of world borders
                    if (w < 0 || w >= World.Instance.GetWidth() ||
                        h < 0 || h >= World.Instance.GetHeight())
                    {
                        return false;
                    }

                    //check if grid square is available / valid
                    if(World.Instance.tiles[w, h].TileType == TileType.BLOCKED)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static Vector2 OffsetToBuildingCenter(Vector2 inPosition, Vector2 inSize)
        {
            return new Vector2(inPosition.x - (int)(0.5f * inSize.x), inPosition.y - (int)(0.5f * inSize.y));
        }

        public static Vector2 PositionToTileCenter(Vector2 inPosition)
        {
            return inPosition + new Vector2(TileSize * 0.5f, TileSize * 0.5f);
        }

        public static Vector2 BuildingCenter(Vector2 inPosition, Vector2 inSize)
        {
            return new Vector2(inPosition.x + (0.5f * inSize.x), inPosition.y + (0.5f * inSize.y));
        }

        public static Vector2 GridToIsometric(Vector2 inGridPosition)
        {
            var isometricPosition = new Vector2(
                (inGridPosition.x - inGridPosition.y) + (World.Instance.GetWidth()),
                ((inGridPosition.x + inGridPosition.y) * 0.5f) + (World.Instance.GetHeight() * 0.5f));

            return isometricPosition;
        }

        public static Vector2 IsometricToGrid(Vector2 inIsoPosition)
        {
            var gridPosition = new Vector2(
                (((2 * inIsoPosition.y) + inIsoPosition.x) * 0.5f) - (World.Instance.GetWidth()),
                ((2 * inIsoPosition.y) - inIsoPosition.x) * 0.5f);

            return gridPosition;
        }

        //ASSUMES BUILDINGS ARE ALWAYS SQUARE
        public static Vector2? GetClosestFreePosition(Vector2 inStartPosition, int minRange = 0, int maxRange = 10)
        {
            //var tileCenterofset = 
            //starting at the buildings position, 
            int range = minRange;
            Vector2? freePosition = null;
            while ((freePosition == null || freePosition.Value == null) && range < maxRange)
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

        public static Vector2? GetTilePositionIfFree(Vector2 inPosition)
        {
            var tileCenter = GridHelper.PositionToTileCenter(inPosition);

            if (!TileIsPassable(inPosition))
            {
                return null;
            }

            var allGameObjects = World.all_worldEntity.AsReadOnly().Select(x => x.ViewObject.GetUnityGameObject()).ToList();

            foreach (var obj in allGameObjects)
            {
                var box = obj.GetComponent<BoxCollider2D>();
                if (box != null)
                {
                    if (box.OverlapPoint(tileCenter))
                    {
                        return null;
                    }
                }
            }
            return tileCenter;
        }

        public static bool TileIsPassable(Vector2 inPosition)
        {
            if (World.Instance.GetTileAt(inPosition).TileType == TileType.BLOCKED)
            {
                return false;
            }
            return true;
        }
	}
}
