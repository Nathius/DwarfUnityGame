using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;
using Assets.UnityWrappers;
using Assets.Models.Buildings;
using Assets.Models.AI;
using Assets.Scripts;

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
            
            Vector2 directionOfTravel = new Vector2(0, 0);

            //if the ai gives a target position, seek towards it
            if (TargetPosition != null)
            {
                directionOfTravel = TargetPosition.Value - this.Position;
            }

            Move(inTimeDelta, directionOfTravel);

            base.Update(inTimeDelta);
        }

        private void Move(float inTimeDelta, Vector2 inDirection)
        {
            var personalSpaceSeeking = GetPersonalSpaceForce();
            var resultDirection = inDirection;
            resultDirection.Normalize();

            resultDirection += personalSpaceSeeking;

            var speedThisFrame = (MoveSpeed * inTimeDelta);
            var movement = resultDirection * speedThisFrame;

            var xPos = Position.x + movement.x;
            var yPos = Position.y + movement.y;
            if(World.Instance.GetTileAt(new Vector2(xPos, Position.y)).TileType == TileType.BLOCKED)
            {
                movement = new Vector2(0, movement.y);
            }

            if (World.Instance.GetTileAt(new Vector2(Position.x, yPos)).TileType == TileType.BLOCKED)
            {
                movement = new Vector2(movement.x, 0);
            }

            this.Position += movement;
        }

        private Vector2 GetPersonalSpaceForce()
        {
            //assume the sprite is square for all units
            var spriteSize = ViewObject.GetSpriteSize();
            var radius = spriteSize != null ? (spriteSize.Value.x) : 1;
            //add 10% for spacing
            var personalSpace = radius;

            var nearbyEntities = World.Instance.EntitiesNearPosition(Position, personalSpace)
                .Where(x => typeof(Tile) != x.GetType())
                .Where(x => x.EntityId != this.EntityId).ToList();

            //build an avoidance vector
            if(nearbyEntities.Count <= 0)
            {
                return new Vector2(0, 0);
            }

            Vector2 avoidanceDirection = new Vector2(0, 0);

            foreach (var entity in nearbyEntities)
            {
                //find vector to next particle from current
                Vector2 toNextParticle = Position - entity.Position;
                var distanceToEntityEdge = toNextParticle.magnitude - (entity.ViewObject.GetSpriteSize().Value.x);
                //find the relative distance to move away from the particle
                Vector2 relativeAvoid = (personalSpace - distanceToEntityEdge) * (toNextParticle);
                //add this new avoid to the result vector
                avoidanceDirection += toNextParticle;
            }
            return avoidanceDirection;
        }

        public void DrawPath(List<Vector2> inPath)
        {
            var lineRenderer = ViewObject.GetUnityGameObject().GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.enabled = true;
            lineRenderer.material.color = Color.cyan;
            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector3(Position.x, Position.y, -1));

            foreach (var point in inPath)
            {
                points.Add(new Vector3(point.x, point.y, -1));
            }
            lineRenderer.SetVertexCount(points.Count);
            lineRenderer.SetWidth(0.2f, 0.2f);
            lineRenderer.SetPositions(points.ToArray());
        }

        public void ClearPath()
        {
            var lineRenderer = ViewObject.GetUnityGameObject().GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.enabled = false;
        }

        public override string ToString()
        {
            var display = "Unit entity " + "\n" +
                " Position: " + VectorHelper.ToString(Position) + "\n";
            if(TargetPosition != null)
            {
                display += " Target: " + VectorHelper.ToString(TargetPosition.Value) + "\n";
            }
            return display;
        }

	}
}
