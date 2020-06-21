using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.UnityWrappers;
using UnityEngine;
using Assets.Models.Buildings;
using Assets.Scripts;

namespace Assets.Models.Econemy.ResourceNodes
{
    public class ResourceNode : Building
	{
        private int ammountRemaining;
        private int depletedRate = 0;
        private bool IsDepeleted { get; set; }
        public RESOURCE_TYPE Resource { get; set; }

        public ResourceNode(UnityObjectWrapper viewObject, Vector2 inPosition, int inTeam, RESOURCE_TYPE inResource, int startingAmmount)
            : base(viewObject, inPosition, inTeam, BuildingType.RESOURCE_NODE)
        {
            Resource = inResource;
            ammountRemaining = startingAmmount;
        }

        public bool CanExtractResource(int inAmmount)
        {
            if (IsDepleted())
            {
                return depletedRate >= inAmmount;
            }
            else
            {
                return (ammountRemaining >= inAmmount);
            }
        }

        public int extractResource(int ammount)
        {
            if (ammountRemaining >= ammount)
            {
                ammountRemaining -= ammount;
                return ammount;
            }
            else if (ammountRemaining > 0)
            {
                int allRemaining = ammountRemaining;
                ammountRemaining = 0;
                return allRemaining;
            }

            return depletedRate;
        }

        public bool IsDepleted()
        {
            if(ammountRemaining <= 0)
            {
                IsDepeleted = true;
                ViewObject.SetColour(Color.red);
            }
            return IsDepeleted;
        }

        public static ResourceNode ClosestNodeToPoint(Vector2 inPosition, float inDistance, RESOURCE_TYPE inResourceType)
        {
            var closestNode = World.all_worldEntity
                .Where(x => x is ResourceNode)
                .Where(x => ((ResourceNode)x).Resource == inResourceType)
                .OrderBy(x => ((ResourceNode)x).IsDepeleted)
                .ThenBy(x => Vector2.Distance(((ResourceNode)x).ReferencePosition(), inPosition))
                .FirstOrDefault();

            ResourceNode node = (ResourceNode)closestNode;

            if (node != null)
            {
                if (Vector2.Distance(node.ReferencePosition(), inPosition) <= inDistance)
                {
                    return node;
                }
            }
            return null;
        }

        public static List<ResourceNode> NodesWithinProximityOfPoint(Vector2 inPosition, float inDistance, RESOURCE_TYPE inResourceType)
        {
            var nodeList = World.all_worldEntity
                .Where(x => x is ResourceNode)
                .Where(x => ((ResourceNode)x).Resource == inResourceType)
                .Where(x => ((ResourceNode)x).IsDepeleted == false)
                .Where(x => Vector2.Distance(((ResourceNode)x).ReferencePosition(), inPosition) <= inDistance)
                .OrderBy(x => Vector2.Distance(((ResourceNode)x).ReferencePosition(), inPosition))
                .Select(x => (ResourceNode)x)
                .ToList<ResourceNode>();

            return nodeList;
        }

        public override string ToString()
        {
            return "Building " + BuildingType.ToString() + "\n" +
                ammountRemaining + " " + Resource.ToString() + "\n" +
                " Position: " + VectorHelper.ToString(Position) + "\n" +
                " Size: " + VectorHelper.ToString(Size) + "\n";
        }
	}
}
