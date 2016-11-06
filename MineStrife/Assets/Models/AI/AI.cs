using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using UnityEngine;
using Assets.Models.AI.PathFinding;

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

        public Unit Body { get; set; }

        public AI(List<CommandTypes> inSupportedBehaviours)
        {
            SupportedBehaviours = inSupportedBehaviours;
            QuedCommands = new List<Command>();
            CurrentPath = null;
            CurrentCommand = null;
        }

        public void Update()
        {
            if(CurrentPath != null && CurrentPath.Any())
            {
                Body.DrawPath(CurrentPath.ToList());
            }
            

            if(CurrentCommand != null)
            {
                //check if command is finished
                switch(CurrentCommand.CommandType)
                {
                    case CommandTypes.MOVE:
                        //move tracks towards a position
                        if (CurrentCommand.TargetPosition != null)
                        {
                            if (CurrentPath == null)
                            {
                                UpdateCurrentPath(Body.Position);
                            }
                            else
                            {
                                MoveAllongPath();
                            }
                        }
                        else
                        {
                            FinishCurrentCommand();
                        }

                        break;
                    case CommandTypes.STOP:
                        //stop clears the current command list
                        break;
                    case CommandTypes.PATROLE:
                        //patrole is a pair of repeated move commands
                        break;
                    case CommandTypes.HOLD:
                        //hold requires no action but never finishes
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

        private void MoveAllongPath()
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
                    FinishCurrentCommand();
                }
            }
        }

        private void UpdateCurrentPath(Vector2 inCurrentPosition)
        {
            var pathingEngin = new PathFinder_AStar(World.Instance.Width, World.Instance.Height, World.Instance.tiles, false);
            var path = pathingEngin.findPath(World.Instance.GetTileAt(inCurrentPosition),
                World.Instance.GetTileAt((int)CurrentCommand.TargetPosition.Value.x, (int)CurrentCommand.TargetPosition.Value.y));
            if (path == null)
            {
                Debug.Log("No path found");
                FinishCurrentCommand();
            }
            else
            {
                path.Reverse();
                Debug.Log("Found path length: " + path.Count);
                var smoothPath = PathSmoother.SmoothPath(path);
                Debug.Log("Smoothed path to: " + smoothPath.Count);
                CurrentPath = smoothPath.Select(x => x.Position).ToList();
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
