﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts;

namespace Assets.Models.AI.PathFinding
{
	public class PathSmoother
	{
        public static List<Vector2> SmoothPath(List<Vector2> inPath)
        {
            //set currentSegmentStart as start of list
            List<Vector2> smoothedPath = new List<Vector2>();
            int currentIndex = 0;

            smoothedPath.Add(inPath[currentIndex]);

            int loopCounter = 0;
            int maxIterations = inPath.Count;
            while (((currentIndex + 1) < inPath.Count) && (loopCounter < maxIterations))
            {
                int nextSegmentEndIndex = GetNextSegmentEndIndex(currentIndex, inPath);
                smoothedPath.Add(inPath[nextSegmentEndIndex]);
                currentIndex = nextSegmentEndIndex;
                loopCounter++;
            }

            return smoothedPath;
        }

        public static int GetNextSegmentEndIndex(int startIndex, List<Vector2> inPath)
        {
            bool pathClear = true;
            int lastValidIndex = startIndex;
            for (int i = startIndex; (i < inPath.Count && pathClear); i++)
            {
                //find the end of the next segment
                pathClear = CanPathSegment(inPath[startIndex], inPath[i]);
                if(pathClear)
                {
                    lastValidIndex = i;
                }
            }
            return lastValidIndex;
        }

        private static bool CanPathSegment(Vector2 inStart, Vector2 inEnd)
        {
            var buildingMask = ConvertLayerToLayerMask("Buildings");
            var tileMask = ConvertLayerToLayerMask("Tiles");

            var finalMask = buildingMask | tileMask;

            //check if the ray passes over any tile which is blocked
            bool foundCollisions = Physics2D.Linecast(
                inStart,
                inEnd,
                finalMask
                );

            bool canPath = !foundCollisions;

            return canPath;
        }

        private static int ConvertLayerToLayerMask(string LayerName)
        {
            //http://answers.unity3d.com/questions/8715/how-do-i-use-layermasks.html
            var layer = LayerMask.NameToLayer(LayerName);
            var layerMask = 1 << layer;
            return layerMask;
        }
	}
}
