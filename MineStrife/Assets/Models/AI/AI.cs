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
        private List<Tile> CurrentPath { get; set; }
        private const float ArrivalDistance = 0.1f;

        public AI(List<CommandTypes> inSupportedBehaviours)
        {
            SupportedBehaviours = inSupportedBehaviours;
            QuedCommands = new List<Command>();
            CurrentPath = null;
            CurrentCommand = null;
        }

        public void Update(Unit inBody)
        {
            if(CurrentPath != null && CurrentPath.Any())
            {
                inBody.DrawPath(CurrentPath.Select(x => x.Position).ToList());
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
                                UpdateCurrentPath(new Vector2((int)inBody.Position.x, (int)inBody.Position.y));
                            }
                            else
                            {
                                MoveAllongPath(inBody);
                                
                            }
                        }
                        else
                        {
                            FinishCurrentCommand(inBody);
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
                inBody.TargetPosition = null;
                //if more orders are in the que, continue on with the first
                if(QuedCommands.Any())
                {
                    CurrentCommand = QuedCommands.First();
                    QuedCommands.RemoveAt(0);
                }
            }

        }

        private void MoveAllongPath(Unit inBody)
        {
            //find the next position in the path to the target
            inBody.TargetPosition = CurrentPath.First().Position;
            Vector2 directionOfTravel = CurrentPath.First().Position - inBody.Position;
            var distance = directionOfTravel.magnitude;

            if (distance <= ArrivalDistance)
            {
                CurrentPath.RemoveAt(0);
                if (CurrentPath.Count >= 1)
                {
                    inBody.TargetPosition = CurrentPath.First().Position;
                }
                else
                {
                    FinishCurrentCommand(inBody);
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
            }
            else
            {
                path.Reverse();
                var smoothPath = PathSmoother.SmoothPath(path);
                CurrentPath = smoothPath;
                //CurrentPath = path;
            }
        }

        private void FinishCurrentCommand(Unit inBody)
        {
            if(CurrentCommand != null)
            {
                if(CurrentCommand.Repeat == true)
                {
                    QuedCommands.Add(CurrentCommand);
                }
                CurrentCommand = null;
                inBody.TargetPosition = null;
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
