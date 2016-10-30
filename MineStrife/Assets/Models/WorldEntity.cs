using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.UnityWrappers;

namespace Assets.Models
{
	public class WorldEntity
	{
        //positions within the world
        public Vector2 Position { get; set; }

        public UnityObjectWrapper ViewObject { get; set; }

        public WorldEntity(UnityObjectWrapper viewObject, Vector2 inPosition)
        {
            World.all_worldEntity.Add(this);
            ViewObject = viewObject;
            Position = inPosition;
        }

        public virtual void Update(float inTimeDelta)
        {
            if (ViewObject != null)
            {
                ViewObject.SetPosition(Position);
            }
        }

        public override string ToString()
        {
            return "World entity " +  "\n" +
                " (" + Math.Round(Position.x, 2) + "," + Math.Round(Position.y, 2) + ")";
        }
	}
}
