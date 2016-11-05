using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;
using Assets.UnityWrappers;
using Assets.Models.Buildings;
using Assets.Models.AI;

namespace Assets.Units
{
	public class Unit : TeamEntity
	{
        public Vector2? TargetPosition { get; set; }
        public UnitType UnitType { get; set; }
        public AI Ai { get; set; }

        public int Health { get; set; }
        public float MoveSpeed { get; set; }

        public Unit(UnityObjectWrapper viewObject, Vector2 inPosition, UnitType inUnitType, AI inAi)
            : base(viewObject, inPosition)
        {
            UnitType = inUnitType;
            MoveSpeed = 2.2f;
            Ai = inAi;
        }

        public override void Update(float inTimeDelta)
        {
            Ai.Update(this);
            
            //if the ai gives a target position, seek towards it
            if (TargetPosition != null)
            {
                Vector2 directionOfTravel = TargetPosition.Value - this.Position;
                directionOfTravel.Normalize();
                var speedThisFrame = (MoveSpeed * inTimeDelta);
                var movement = directionOfTravel * speedThisFrame;
                this.Position += movement;
            }
           
            base.Update(inTimeDelta);
        }

        public override string ToString()
        {
            var display = "Unit entity " + "\n" +
                " (" + Math.Round(Position.x, 2) + "," + Math.Round(Position.y, 2) + ") \n";
            if(TargetPosition != null)
            {
                display += " (" + Math.Round(TargetPosition.Value.x, 2) + "," + Math.Round(TargetPosition.Value.y, 2) + ")";
            }
            return display;
        }

	}
}
