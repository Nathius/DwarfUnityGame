using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts;

namespace Assets.Models.AI.PathFinding
{
	public class PathSmoother
	{
        public static List<Tile> SmoothPath(List<Tile> inPath)
        {
            //set currentSegmentStart as start of list
            List<Tile> smoothedPath = new List<Tile>();
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

            Debug.Log("smoothed path length: " + smoothedPath.Count);
            return smoothedPath;
        }

        public static int GetNextSegmentEndIndex(int startIndex, List<Tile> inPath)
        {
            bool pathClear = true;
            int lastValidIndex = startIndex;
            for (int i = startIndex; (i < inPath.Count && pathClear); i++)
            {
                //find the end of the next segment
                pathClear = CanPathSegment(inPath[startIndex].Position, inPath[i].Position);
                if(pathClear)
                {
                    lastValidIndex = i;
                }
            }
            return lastValidIndex;
        }

        private static bool CanPathSegment(Vector2 inStart, Vector2 inEnd)
        {
            RaycastHit hitInfo = new RaycastHit();

            //check if the ray passes over any tile which is blocked
            bool foundCollisions = Physics2D.Linecast(
                inStart,
                inEnd
                );

            bool canPath = !foundCollisions;
            Debug.Log("Can path: " + canPath.ToString() + " Hit point: " + hitInfo.point + " at distance " + hitInfo.distance);

            return canPath;
        }

	}
}
