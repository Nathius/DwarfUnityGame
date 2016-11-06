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
            Ai.Body = this;
        }

        public override void Update(float inTimeDelta)
        {
            Ai.Update();
            
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

        public void DrawPath(List<Vector2> inPath)
        {
            var lineRenderer = ViewObject.GetUnityGameObject().GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.material.color = Color.cyan;
                List<Vector3> points = new List<Vector3>();
                points.Add(new Vector3(Position.x, Position.y, 1));

                foreach (var point in inPath)
                {
                    points.Add(new Vector3(point.x, point.y, 1));
                }
                lineRenderer.SetVertexCount(points.Count);
                lineRenderer.SetWidth(0.2f, 0.2f);
                lineRenderer.SetPositions(points.ToArray());
                
            }
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
