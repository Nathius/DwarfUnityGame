using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
	public class FormationCalculator
	{
        public static List<Vector2> findPositionsForFormation(Vector2 inHeading, int inNumUnits, Vector2 inTargetPosition, float inAvgUnitSize)
        {
            List<Vector2> posList = new List<Vector2>();
            float rootNum = (float)Math.Sqrt(inNumUnits);

            //ratio is the width / length, eg r = 2 is twice as wide as long
            var formationRatio = 0.5f;
            int width = (int)Math.Round(rootNum + (rootNum * formationRatio));
            if (width <= 0)
            {
                width = 1;
            }

            int length = (int)Math.Ceiling((float)inNumUnits / (float)width);

            //Approximate width of formation
            float formationWidth = (width - 1) * (inAvgUnitSize);

            float widthBorder = (formationWidth / 2.0f);

            Vector2 nextPosition = inTargetPosition - (new Vector2(widthBorder, 0));
            int placed = 0;

            //create formation positions at normal angle
            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    posList.Add(nextPosition);
                    placed++;
                    nextPosition.x += inAvgUnitSize;
                }
                nextPosition.x = inTargetPosition.x - widthBorder;
                nextPosition.y += inAvgUnitSize;
            }

            if (placed < inNumUnits)
            {
                int distRadius = (int)(formationWidth / (inNumUnits - placed + 1));
                nextPosition.x = inTargetPosition.x - widthBorder;
                for (int i = placed; i < inNumUnits; i++)
                {
                    nextPosition.x += distRadius;
                    posList.Add(nextPosition);
                }
            }

            //rotate each position by the heading angle
            for (int i = 0; i < posList.Count; i++)
            {
                Vector2 clickToPos = posList[i] - inTargetPosition;
                float angle = VectorHelper.angleBetween(new Vector2(0, -1), inHeading);
                //check for direction of angle
                if (inHeading.x < 0)
                {
                    angle *= -1;
                }
                posList[i] = inTargetPosition + VectorHelper.rotateVector2(clickToPos, angle);
            }

            return posList;
        }

        public static List<Vector2> findPositionsForFormation(List<Vector2> inStartPositions,Vector2 inTargetPosition, float inAvgUnitSize)
        {
            var heading = FindHeading(inStartPositions, inTargetPosition);
            return findPositionsForFormation(heading, inStartPositions.Count(), inTargetPosition, inAvgUnitSize);
        }

        public static Vector2 FindHeading(List<Vector2> inPositions, Vector2 inTargetPosition)
        {
            //find average unit position
            Vector2 averageUnitPosition = new Vector2(0, 0);
            int numUnits = 0;
            for (int i = 0; i < inPositions.Count; i++)
            {
                averageUnitPosition += inPositions[i];
                numUnits++;
            }
            averageUnitPosition /= numUnits;

            //find heading to click, and find position list
            return inTargetPosition - averageUnitPosition;
        }
	}
}
