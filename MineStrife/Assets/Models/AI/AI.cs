using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using UnityEngine;
using Assets.Models.AI.PathFinding;
using Assets.Scripts;
using Assets.Models.AI.Routines;

namespace Assets.Models.AI
{
	public class AI
	{
        private List<CommandTypes> SupportedBehaviours { get; set; }
        private CommandManager CommandManager { get; set; }
        public Unit Body { get; set; }
        private Routine currentRoutine { get; set; }

        public AI(List<CommandTypes> inSupportedBehaviours)
        {
            SupportedBehaviours = inSupportedBehaviours;
            CommandManager = new CommandManager();
            currentRoutine = null;
        }

        public void Update(float inTimeDelta)
        {
            //if we have a command
            var currentCommand = CommandManager.GetCurrentcommand();
            if (currentCommand != null)
            {
                //if we know what to do about it, do that
                if (currentRoutine != null)
                {
                    //check for finish
                    if (currentRoutine.GetIsFinished())
                    {
                        currentRoutine = null;
                        currentCommand = null;
                        CommandManager.FinishCurrentCommand();
                    }
                    else
                    {
                        //run routine logic
                        currentRoutine.Update(inTimeDelta);
                    }
                }
                else //pick a routine to complete the command
                {
                    //create a new routine to run untill the command is complete
                    switch (currentCommand.CommandType)
                    {
                        case CommandTypes.MOVE:
                            currentRoutine = new MoveRoutine(Body, currentCommand);
                            break;
                        case CommandTypes.PATROLE: //TODO probably more like attack move than ignore move
                            break;

                        case CommandTypes.STOP:
                            //might not be a logical way to do a stop
                            currentRoutine = new StopRoutine(Body, currentCommand);
                            break;

                        case CommandTypes.HOLD:
                            //hold requires no action but never finishes
                            break;

                        case CommandTypes.BUILD:
                            //hold requires no action but never finishes
                            currentRoutine = new BuildRoutine(Body, currentCommand);
                            break;

                        case CommandTypes.FOLLOW:
                            //move to proximity of unit, never finishes
                            currentRoutine = new FollowRoutine(Body, currentCommand);
                            break;

                        default:
                            //idle or type not found / supported?
                            break;
                    }
                }
                

                //else attempt to action command
            }
        }

        public void AddCommand(Command inCommand, bool inAddToQue)
        {
            //TODO check if we are capable of respoding to that command

            if (SupportsBehaviour(inCommand.CommandType))
            {
                if (inAddToQue)
                {
                    CommandManager.QueCommand(inCommand);
                }
                else
                {
                    CommandManager.SetCurrentCommand(inCommand);
                    currentRoutine = null;
                }
            }
        }

        public bool SupportsBehaviour(CommandTypes inCommandType)
        {
            //TODO move to order manager?
            if(SupportedBehaviours.Contains(inCommandType))
            {
                return true;
            }
            return false;
        }



	}
}
