using System.Collections;
using System;
using Assets.UnityWrappers;
using Assets.Models.Buildings;
using UnityEngine;
using Assets.Scripts;

namespace Assets.Models
{

    public class Building : TeamEntity
    {
        private static readonly Vector2 DefaultSize = new Vector2(2, 2);
        public BuildingType BuildingType { get; set; }
        public Vector2 Size { get; set; }

        public Building(UnityObjectWrapper viewObject,
            Vector2 inPosition,
            BuildingType inBuildingType)
            : base(viewObject, inPosition)
        {
            BuildingType = inBuildingType;
            SetBuildingSize();

            viewObject.SetBoundSize(Size);
            GridHelper.AddBuildingToCollisionMap(inPosition, Size);
        }

        public Vector2 ReferencePosition()
        {
            return GridHelper.BuildingCenter(Position, Size);
        }

        private void SetBuildingSize()
        {
            var definition = BuildingDefinition.GetBuildingDefinitionForType(BuildingType);
            if (definition != null)
            {
                Size = definition.Size;
            }
            else
            {
                Size = DefaultSize;
            }
        }

        public override string ToString()
        {
            return "Building " + BuildingType.ToString() + "\n" +
                " Position: " + VectorHelper.ToString(Position) + "\n" +
                " Size: " + VectorHelper.ToString(Size) + "\n";
        }
    }
}
