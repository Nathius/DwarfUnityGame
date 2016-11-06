using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;

namespace Assets.Scripts
{
	public static class GridHelper
	{
        public static int TileSize = 1;

        public static Vector2 SnapBuildingToGridPosition(Vector2 inPosition, int inWidth, int inHeight)
        {
            Vector2 clippedPosition = new Vector2((int)inPosition.x, (int)inPosition.y);
            return clippedPosition;
        }

        public static void AddBuildingToCollisionMap(Vector2 inPosition, int inWidth, int inHeight)
        {
            //count up by width and height
            for (int w = (int)inPosition.x; w < (int)(inPosition.x + inWidth); w++)
            {
                for (int h = (int)inPosition.y; h < (int)(inPosition.y + inHeight); h++)
                {
                    World.Instance.tiles[w, h].Cost = 0;
                    World.Instance.tiles[w, h].TileType = TileType.BLOCKED;
                }
            }
        }

        private static float FindLowerOrdinate(float inValue, int inSize)
        {
            //finds the lower edge of a buildings collision box on the grid
            //even size ordinates live on grid intersects, odd sized ordinates live on grid centers
            if (MathHelper.IsEven(inSize))
            {
                var clampedPosition = (int)inValue;
                var centerOfset = (int)((inSize / 2.0f) - 1);
                var lowerOrdinate = inValue - centerOfset;
                return lowerOrdinate;
            }
            else
            {
                var centerOfset = (int)(inSize / 2.0f);
                var lowerOrdinate = inValue - centerOfset;
                return lowerOrdinate;
            }
        }


	}
}
