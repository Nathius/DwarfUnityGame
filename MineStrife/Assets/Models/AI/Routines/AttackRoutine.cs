using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using Assets.Models.AI.Routines.SubRoutine;

namespace Assets.Models.AI.Routines
{
	public class AttackRoutine : Routine
	{
        //private AttackSubRoutine movetoPoint { get; set; }

        public AttackRoutine(Unit inBody, Command inCommand)
            : base(inBody, inCommand)
        {
            //negate command if target position not provided
            if(inCommand.TargetPosition.HasValue == false)
            {
                IsFinished = true;
            }

            //movetoPoint = new MoveToPointSubRoutine(inBody, inCommand.TargetPosition.Value);
        }

        

        public override void Update(float inTimeDelta)
        {
            //exit if routine is finished, manager should pick up and delete routine
            if(IsFinished)
            {
                return;
            }

            //check if subroutine is finished , and if so finish routine
            //if (movetoPoint.GetIsFinished() == true)
            //{
            //    IsFinished = true;
            //    return;
            //}

            ////else allow sub routine to run
            //movetoPoint.Update(inTimeDelta);
        }
	}
}
