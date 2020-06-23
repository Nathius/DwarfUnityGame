using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Models.AI
{
	public class CommandManager
	{
        private static readonly int MaxComments = 10;

        private List<Command> QuedCommands { get; set; }
        private Command CurrentCommand { get; set; }

        public CommandManager()
        {
            QuedCommands = new List<Command>();
            
            CurrentCommand = null;
        }

        public Command GetCurrentcommand()
        {
            //assume commands will pop up to current each time one finishes
            if(CurrentCommand == null)
            {
                checkForQuedCommands();
            }
            return CurrentCommand;
        }

        public void FinishCurrentCommand()
        {
            if (CurrentCommand != null)
            {
                if (CurrentCommand.Repeat == true)
                {
                    QuedCommands.Add(CurrentCommand);
                }
                CurrentCommand = null;
            }
            //else no command to finish

            //check if something in the que
            checkForQuedCommands();
        }

        private void checkForQuedCommands()
        {
            if (QuedCommands.Any())
            {
                CurrentCommand = QuedCommands.First();
                QuedCommands.RemoveAt(0);
            }
        }

        public bool QueCommand(Command newCommand, bool pushPriority = false)
        {
            if(QuedCommands.Count < MaxComments)
            {
                if (pushPriority)
                {
                    //take current command, push it onto the que
                    //and make new command current
                    QuedCommands.Insert(0, CurrentCommand);
                    CurrentCommand = newCommand;
                    //unit should go back to what it was doing afterwards
                }
                else
                {
                    QuedCommands.Add(newCommand);
                }
                
                return true;
            }

            //command que is full
            return false;
        }

        public void SetCurrentCommand(Command newCommand)
        {
            //assume no clean up on current commands
            //blow them all away
            CurrentCommand = null;
            QuedCommands.Clear();

            //add new command 
            CurrentCommand = newCommand;

        }

	}
}
