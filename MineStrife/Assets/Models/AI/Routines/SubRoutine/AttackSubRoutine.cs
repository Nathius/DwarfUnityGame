using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Units;
using Assets.Models.AI.PathFinding;
using Assets.Scripts;
using Assets.Models.Units;

namespace Assets.Models.AI.Routines.SubRoutine
{
    public class AttackSubRoutine
    {
        private const float WayPointArrivalDistance = 0.1f;
        private const float FollowArrivalDistance = 1.5f;
        private const float PathReCalcThreshold = 3f;
        private const float DumbFollowDistance = 4f;

        private List<Vector2> CurrentPath { get; set; }

        private Unit TargetUnit { get; set; }
        private Unit Body { get; set; }

        private bool isFinished;

        public AttackSubRoutine(Unit inBody, Unit inTargetUnit)
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
            if (TargetUnit.GetIsDead())
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


            //if in weapon range
            if (disToTarget <= Body.Weapon.Range)
            {
                //clear pathing
                CurrentPath = null;
                Body.TargetPosition = null;

                //if weapon ready
                if (Body.Weapon.ReadyToFire())
                {
                    //fire weapon
                    Body.Weapon.FireAtUnit(TargetUnit);
                    Body.SetSpriteState(UnitSpriteState.AIMING);
                }
                else
                {
                    //wait
                    //return;
                    Body.SetSpriteState(UnitSpriteState.RELOADING);
                }
            }
            else
            {
                Body.SetSpriteState(UnitSpriteState.MOVING);

                //if has path
                if (CurrentPath != null)
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
                else
                {
                    UpdateCurrentPath(Body.Position, TargetUnit.Position);
                    MoveAllongPath();
                }

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
