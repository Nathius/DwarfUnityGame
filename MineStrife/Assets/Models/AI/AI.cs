using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using UnityEngine;
using Assets.Models.AI.PathFinding;
using Assets.Scripts;

namespace Assets.Models.AI
{
	public class AI
	{
        private List<CommandTypes> SupportedBehaviours { get; set; }
        private List<Command> QuedCommands { get; set; }
        private static readonly int MaxComments = 10;

        private Command CurrentCommand { get; set; }
        private List<Vector2> CurrentPath { get; set; }
        private const float ArrivalDistance = 0.1f;



        private float nextFollowRefresh;

        public Unit Body { get; set; }
        public AiStateMachine StateMachine { get; set; }

        public AI(List<CommandTypes> inSupportedBehaviours)
        {
            SupportedBehaviours = inSupportedBehaviours;
            QuedCommands = new List<Command>();
            CurrentPath = null;
            CurrentCommand = null;

            StateMachine = new AiStateMachine();

            nextFollowRefresh = 0;
        }

        public void Update(float inTimeDelta)
        {
            if (CurrentPath != null && CurrentPath.Any())
            {
                //Body.DrawPath(CurrentPath.ToList());
            }
            else
            {
                //Body.ClearPath();
            }
            

            if(CurrentCommand != null)
            {
                //check if command is finished
                switch(CurrentCommand.CommandType)
                {
                    case CommandTypes.MOVE:
                    case CommandTypes.PATROLE: //TODO probably more like attack move than ignore move
                        //move tracks towards a position
                        if (CurrentCommand.TargetPosition != null)
                        {
                            if (CurrentPath == null)
                            {
                                UpdateCurrentPath(Body.Position, CurrentCommand.TargetPosition);
                            }
                            else
                            {
                                MoveAllongPath();
                            }
                        }
                        else //reached position
                        {
                            FinishCurrentCommand();
                        }

                        break;

                    case CommandTypes.STOP:
                        //stop clears the current command list
                        StopAllCommands();
                        break;

                    case CommandTypes.HOLD:
                        //hold requires no action but never finishes
                        break;

                    case CommandTypes.BUILD:
                        //hold requires no action but never finishes
                        break;

                    case CommandTypes.FOLLOW:
                        if (CurrentCommand.TargetEntity != null)
                        {
                            if (nextFollowRefresh <= 0 || CurrentPath == null)
                            {
                                UpdateCurrentPath(Body.Position, CurrentCommand.TargetEntity.Position, false);
                                nextFollowRefresh = 0.5f;
                            }
                            nextFollowRefresh -= inTimeDelta;

                            if (CurrentPath != null && CurrentPath.Count > 0)
                            {
                                MoveAllongPath(false);
                            }
                        }
                        //never finishes
                        break;

                    default:
                        //idle or type not found / supported?
                        break;
                }

                //else attempt to action command
            }

            if(CurrentCommand == null)
            {
                Body.TargetPosition = null;
                //if more orders are in the que, continue on with the first
                if(QuedCommands.Any())
                {
                    CurrentCommand = QuedCommands.First();
                    QuedCommands.RemoveAt(0);
                }
            }

        }

        private void MoveAllongPath(bool allowFinish = true)
        {
            //find the next position in the path to the target
            Body.TargetPosition = CurrentPath.First();
            Vector2 directionOfTravel = CurrentPath.First() - Body.Position;
            var distance = directionOfTravel.magnitude;

            if (distance <= ArrivalDistance)
            {
                CurrentPath.RemoveAt(0);
                if (CurrentPath.Count >= 1)
                {
                    var targerPosition = CurrentPath.First();
                    Body.TargetPosition = new Vector2(targerPosition.x + 0.5f, targerPosition.y + 0.5f);
                }
                else
                {
                    if (allowFinish)
                    {
                        FinishCurrentCommand();
                    }
                }
            }
        }

        private void UpdateCurrentPath(Vector2 inCurrentPosition, Vector2? targetPosition, bool allowFinish = true)
        {
            var pathingEngin = new PathFinder_AStar(World.Instance.GetWidth(), World.Instance.GetHeight(), World.Instance.tiles, false);

            if (inCurrentPosition == null || targetPosition.Value == null)
            {
                if (allowFinish)
                {
                    FinishCurrentCommand();
                }
                
                return;
            }

            var path = pathingEngin.findPath(inCurrentPosition, targetPosition.Value);
            if (path == null)
            {
                Debug.Log("No path found");
                if (allowFinish)
                {
                    FinishCurrentCommand();
                }
            }
            else
            {
                //ofset tile positions to tile center
                var centeredPath = path.Select(x => GridHelper.PositionToTileCenter(x)).ToList();

                //add the start and end pos back onto the path list
                centeredPath.Insert(0, inCurrentPosition);

                //TODO fix different movement options
                // dont add final position to follow commands
                if (CurrentCommand.CommandType == CommandTypes.FOLLOW && centeredPath.Count >= 1)
                {
                    centeredPath.RemoveAt(centeredPath.Count - 1);
                }
                else
                {
                    centeredPath.Add(targetPosition.Value);
                }
                

                // issues with ray tracing making it difficult to smoth the path
                //var smoothPath = PathSmoother.SmoothPath(centeredPath);
                //Debug.Log("smoothPath " + string.Join(" -> ", smoothPath.Select(x => VectorHelper.ToString(x)).ToArray<string>())); 
                //CurrentPath = smoothPath;

                CurrentPath = centeredPath;
            }
        }

        private void FinishCurrentCommand()
        {
            if(CurrentCommand != null)
            {
                if(CurrentCommand.Repeat == true)
                {
                    QuedCommands.Add(CurrentCommand);
                }
                CurrentCommand = null;
                Body.TargetPosition = null;
            }
            CurrentPath = null;
        }

        private void StopAllCommands()
        {
            //stops current task (if any) and clears queue
            QuedCommands.Clear();
            CurrentCommand = null;
            Body.TargetPosition = null;
            CurrentPath = null;
        }

        public void AddCommand(Command inCommand, bool inAddToQue)
        {
            if (SupportsBehaviour(inCommand.CommandType))
            {
                if (inAddToQue)
                {
                    if (MaxComments > QuedCommands.Count)
                    {
                        QuedCommands.Add(inCommand);
                    }
                    else
                    {
                        //Do not add command to a full que, 
                        //TODO report full que to ui
                    }
                }
                else
                {
                    QuedCommands.Clear();
                    CurrentPath = null;
                    CurrentCommand = inCommand;
                }
            }
            //default to similar behaviour?
        }

        public bool SupportsBehaviour(CommandTypes inCommandType)
        {
            if(SupportedBehaviours.Contains(inCommandType))
            {
                return true;
            }
            return false;
        }



	}
}
