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
    public class FollowUnitSubRoutine
    {
        private const float WayPointArrivalDistance = 0.1f;
        private const float FollowArrivalDistance = 1.5f;
        private const float PathReCalcThreshold = 3f;
        private const float DumbFollowDistance = 4f;

        private List<Vector2> CurrentPath { get; set; }

        private Unit TargetUnit { get; set; }
        private Unit Body { get; set; }

        private bool isFinished;

        public FollowUnitSubRoutine(Unit inBody, Unit inTargetUnit)
        {
            CurrentPath = null;
            Body = inBody;
            TargetUnit = inTargetUnit;
            isFinished = false;

            //on init run pathfinding to set path
            UpdateCurrentPath(Body.Position, inTargetUnit.Position);           
        }

        public bool GetIsFinished()
        {
            return isFinished;
        }

        public void Update(float inTimeDelta)
        {
            if(TargetUnit.isDead)
            {
                isFinished = true;
            }

            if (isFinished)
            {
                //managing routine should pick up the finish and delete the sub routine
                return; 
            }


            //how far are we from our target
            var disToTarget = VectorHelper.getDistanceBetween(TargetUnit.Position, Body.Position);

            //Debug.Log("follow status: distanceTTarget: " + disToTarget + "");

            //if we are currently pathing continue 
            if(CurrentPath != null)
            {
                //and the path ends close enough to the target
                var pathDestinationDistanceToTarget = VectorHelper.getDistanceBetween(CurrentPath.Last(), TargetUnit.Position);
                if (pathDestinationDistanceToTarget < PathReCalcThreshold)
                {
                    //Debug.Log("Follow: has valid path, following");
                    //follow existing path
                    MoveAllongPath();
                }
                else
                {
                    //Debug.Log("Follow: has stale path, updating and following");
                    //refresh with new path
                    UpdateCurrentPath(Body.Position, TargetUnit.Position);
                    MoveAllongPath();
                }
            }
            //else if we are further than the dumb follow distance
            else if (disToTarget >= DumbFollowDistance)
            {
                //Debug.Log("no path, but further than dumb follow, find path and move");
                //refresh with new path
                UpdateCurrentPath(Body.Position, TargetUnit.Position);
                MoveAllongPath();
            }
            //else if we are within the dumb follow distance
            else if (FollowArrivalDistance < disToTarget && disToTarget <= DumbFollowDistance)
            {
                //Debug.Log("not close enough to target, and within dumb follow, so dumb follow");
                // track straight towards the target
                CurrentPath = null;
                Body.TargetPosition = TargetUnit.Position;
            }
            else
            {
                //Debug.Log("close enough to target, just idle");
                //close enough the targert unit, just idle and wait
                CurrentPath = null;
                Body.TargetPosition = null;
            }
               
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

        private void MoveAllongPath()
        {
            //find the next position in the path to the target
            Body.TargetPosition = CurrentPath.First();
            var distance = VectorHelper.getDistanceBetween(CurrentPath.First(), Body.Position);

            //if we have arrived at our next waypoint
            if (distance <= WayPointArrivalDistance)
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
                    CurrentPath = null;
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
                return;
            }

            //ofset tile positions to tile center
            var centeredPath = path.Select(x => GridHelper.PositionToTileCenter(x)).ToList();

            //add the start and end pos back onto the path list
            centeredPath.Insert(0, inCurrentPosition);
            centeredPath.Add(targetPoint);

            CurrentPath = centeredPath;
        }
    }
}
