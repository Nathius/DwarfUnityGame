﻿using System;
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
            for (int w = (int)inPosition.x; (w < (int)(inPosition.x + inSize.x)) && (w < World.Instance.Width); w++)
            {
                for (int h = (int)inPosition.y; (h < (int)(inPosition.y + inSize.y)) && (h < World.Instance.Height); h++)
                {
                    World.Instance.tiles[w, h].Cost = 0;
                    World.Instance.tiles[w, h].TileType = TileType.BLOCKED;
                }
            }
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
	}
}
