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
        private AttackSubRoutine attackSub { get; set; }

        public AttackRoutine(Unit inBody, Command inCommand)
            : base(inBody, inCommand)
        {
            //negate command if target position not provided
            if (inCommand.TargetEntity == null)
            {
                IsFinished = true;
                return;
            }

            if((inCommand.TargetEntity is Unit) == false)
            {
                IsFinished = true;
                return;
            }

            var targetUnit = (Unit)inCommand.TargetEntity;
            attackSub = new AttackSubRoutine(inBody, targetUnit);
        }

        

        public override void Update(float inTimeDelta)
        {
            //exit if routine is finished, manager should pick up and delete routine
            if(IsFinished)
            {
                return;
            }

            //check if subroutine is finished , and if so finish routine
            if (attackSub.GetIsFinished() == true)
            {
                IsFinished = true;
                return;
            }

            //else allow sub routine to run
            attackSub.Update(inTimeDelta);
        }
	}
}
