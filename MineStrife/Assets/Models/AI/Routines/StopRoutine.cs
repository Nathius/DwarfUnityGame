using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;

namespace Assets.Models.AI.Routines
{
	public class StopRoutine : Routine
	{
        public StopRoutine(Unit inBody, Command inCommand)
            : base(inBody, inCommand)
        {

        }

        public override void Update(float inTimeDelta)
        {
            //exit if routine is finished, manager should pick up and delete routine
            if (IsFinished)
            {
                return;
            }

            //force stop a unit
            Body.TargetPosition = null;
            IsFinished = true;

        }
	}
}
