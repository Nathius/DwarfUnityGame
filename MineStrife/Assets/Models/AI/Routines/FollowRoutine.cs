using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using Assets.Models.AI.Routines.SubRoutine;
using UnityEngine;
using Assets.Models.Buildings;

namespace Assets.Models.AI.Routines
{
	public class FollowRoutine : Routine
	{
        private Unit TargetUnit { get; set; }

        private FollowUnitSubRoutine followSubRoutine { get; set; }

        public FollowRoutine(Unit inBody, Command inCommand)
            : base(inBody, inCommand)
        {
            //negate command if target entity not provided
            if(inCommand.TargetEntity == null)
            {
                IsFinished = true;
                return;
            }


            if ((inCommand.TargetEntity is Unit) == false)
            {
                IsFinished = true;
                return;
            }

            TargetUnit = (Unit)inCommand.TargetEntity;

            followSubRoutine = new FollowUnitSubRoutine(inBody, TargetUnit);
        }

        

        public override void Update(float inTimeDelta)
        {
            //exit if routine is finished, manager should pick up and delete routine
            if(IsFinished)
            {
                return;
            }

            if (followSubRoutine.GetIsFinished())
            {
                IsFinished = true;
                return;
            }

            followSubRoutine.Update(inTimeDelta);         
        }
	}
}
