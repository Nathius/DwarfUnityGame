using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.UnityWrappers;
using Assets.Scripts;

namespace Assets.Models
{
	public class WorldEntity
	{
        //positions within the world
        public int EntityId { get; set; }
        public static int NextId { get; set; }

        public Vector2 Position { get; set; }

        public UnityObjectWrapper ViewObject { get; set; }

        public WorldEntity(UnityObjectWrapper viewObject, Vector2 inPosition)
        {
            EntityId = NextId;
            NextId++;

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
            return "World entity " + "\n" +
                " Position: " + VectorHelper.ToString(Position) + "\n";
        }
	}
}
