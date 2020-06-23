using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;

namespace Assets.Models.AI.Routines
{
	public abstract class Routine
	{
        protected bool IsFinished { get; set; }
        protected Command Command { get; set; }
        protected Unit Body { get; set; }

        public Routine(Unit inBody, Command inCommand)
        {
            IsFinished = false;
            Command = inCommand;
        }

        public bool GetIsFinished()
        {
            return IsFinished;
        }

        //update method that needs to be defined in any sub class
        public abstract void Update(float inTimeDelta);
	}
}
