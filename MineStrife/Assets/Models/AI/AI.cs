using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using UnityEngine;

namespace Assets.Models.AI
{
	public class AI
	{
        private List<CommandTypes> SupportedBehaviours { get; set; }
        private List<Command> QuedCommands { get; set; }
        private static readonly int MaxComments = 10;

        private Command CurrentCommand { get; set; }

        public AI(List<CommandTypes> inSupportedBehaviours)
        {
            SupportedBehaviours = inSupportedBehaviours;
            QuedCommands = new List<Command>();
            CurrentCommand = null;
        }

        public void Update(Unit inBody)
        {
            if(CurrentCommand != null)
            {
                //check if command is finished
                switch(CurrentCommand.CommandType)
                {
                    case CommandTypes.MOVE:
                        //move tracks towards a position
                        //should use pathing
                        if (CurrentCommand.TargetPosition != null)
                        {
                            //find the next position in the pathing to the target
                            Vector2 directionOfTravel = CurrentCommand.TargetPosition.Value - inBody.Position;
                            var distance = directionOfTravel.magnitude;
                            if (distance > 0.1f)
                            {
                                //should pick the next position in the path list
                                inBody.TargetPosition = CurrentCommand.TargetPosition;
                            }
                            else
                            {
                                FinishCurrentCommand(inBody);
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
                //if more orders are in the que, continue on with the first
                if(QuedCommands.Any())
                {
                    CurrentCommand = QuedCommands.First();
                    QuedCommands.RemoveAt(0);
                }
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
