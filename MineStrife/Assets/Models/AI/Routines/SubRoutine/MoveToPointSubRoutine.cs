using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Units;
using Assets.Models.AI.PathFinding;
using Assets.Scripts;

namespace Assets.Models.AI.Routines.SubRoutine
{
	public class MoveToPointSubRoutine
	{
        private const float ArrivalDistance = 0.1f;

        private List<Vector2> CurrentPath { get; set; }

        private Vector2 TargetPoint { get; set; }
        private Unit Body { get; set; }

        private bool isFinished;

        public MoveToPointSubRoutine(Unit inBody, Vector2 inTargetPoint)
        {
            CurrentPath = null;
            Body = inBody;
            TargetPoint = inTargetPoint;
            isFinished = false;

            //on init run pathfinding to set path
            UpdateCurrentPath(Body.Position, TargetPoint);
        }

        public bool GetIsFinished()
        {
            return isFinished;
        }

        public void Update(float inTimeDelta)
        {
            if (isFinished)
            {
                //managing routine should pick up the finish and delete the sub routine
                return; 
            }

            MoveAllongPath();

            if(DebugFlags.showUnitPathfingindPaths)
            {
                if (CurrentPath != null && CurrentPath.Any())
                {
                    Body.DrawPath(CurrentPath.ToList());
                }
                else
                {
                    Body.ClearPath();
                }
            }
        }

        private void MoveAllongPath(bool allowFinish = true)
        {
            //find the next position in the path to the target
            Body.TargetPosition = CurrentPath.First();
            var distance = VectorHelper.getDistanceBetween(CurrentPath.First(), Body.Position);

            //if we have arrived at our next waypoint
            if (distance <= ArrivalDistance)
            {
                //remove the way point
                CurrentPath.RemoveAt(0);

                //if we have more waypoints
                if (CurrentPath.Count >= 1)
                {
                    //target the next waypoint
                    var targerPosition = CurrentPath.First();
                    Body.TargetPosition = new Vector2(targerPosition.x + 0.5f, targerPosition.y + 0.5f);
                }
                else
                {
                    //else no more waypoints, move is finished
                    Body.TargetPosition = null;
                    isFinished = true;
                }
            }
        }

        private void UpdateCurrentPath(Vector2 inCurrentPosition, Vector2 targetPoint)
        {
            //init pathing engin
            var pathingEngin = new PathFinder_AStar(World.Instance.GetWidth(), World.Instance.GetHeight(), World.Instance.tiles, false);

            var path = pathingEngin.findPath(inCurrentPosition, targetPoint);
            if (path == null)
            {
                Debug.Log("No path found");
                isFinished = true;
                return;
            }
            
            //ofset tile positions to tile center
            var centeredPath = path.Select(x => GridHelper.PositionToTileCenter(x)).ToList();

            //add the start and end pos back onto the path list
            centeredPath.Insert(0, inCurrentPosition);
            centeredPath.Add(targetPoint);

            //TODO below comment block was for formations, which will need to be totally
            // redesigned to work with the new subroutine pattern

            // issues with ray tracing making it difficult to smoth the path
            //var smoothPath = PathSmoother.SmoothPath(centeredPath);
            //Debug.Log("smoothPath " + string.Join(" -> ", smoothPath.Select(x => VectorHelper.ToString(x)).ToArray<string>())); 
            //CurrentPath = smoothPath;

            CurrentPath = centeredPath;
        }
	}
}
