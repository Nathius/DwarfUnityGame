using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;
using Assets.UnityWrappers;
using Assets.Models.Buildings;

namespace Assets.Units
{
	public class Unit : TeamEntity
	{
        public Vector2? TargetPosition { get; set; }
        public UnitType UnitType { get; set; }

        public int Health { get; set; }
        public float MoveSpeed { get; set; }

        public Unit(UnityObjectWrapper viewObject, Vector2 inPosition, UnitType inUnitType)
            : base(viewObject, inPosition)
        {
            UnitType = inUnitType;
            MoveSpeed = 0.02f;
        }

        public void SetTargetPosition(Vector2 inTargetPosition)
        {
            TargetPosition = inTargetPosition;
        }

        public override void Update(float inTimeDelta)
        {
            //target seeking behaviour
            if (TargetPosition != null)
            {
                Vector2 directionOfTravel = TargetPosition.Value - this.Position;
                var distance = directionOfTravel.magnitude;
                if (distance <= 0.1f)
                {
                    TargetPosition = null;
                }
                else
                {
                    directionOfTravel.Normalize();
                    var movement = directionOfTravel * MoveSpeed;
                    this.Position += movement;
                }


            }
           


            base.Update(inTimeDelta);
        }

	}
}
