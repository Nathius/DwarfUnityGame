using System.Collections;
using System;
using Assets.UnityWrappers;
using Assets.Models.Buildings;
using UnityEngine;

namespace Assets.Models
{

    public class Building : TeamEntity
    {
        public int tileWidth { get; set; }
        public int tileHeight { get; set; }
        public BuildingType BuildingType { get; set; }

        public Building(UnityObjectWrapper viewObject,
            Vector2 inPosition,
            int inWidth, 
            int inHeight, 
            BuildingType inBuildingType)
            : base(viewObject, inPosition)
        {
            tileWidth = inWidth;
            tileHeight = inHeight;
            BuildingType = inBuildingType;
            viewObject.SetBoundSize(inWidth, inHeight);
        }

        public override string ToString()
        {
            return "Building " + BuildingType.ToString() + "\n" +
                " (" + Math.Round(Position.x, 2) + "," + Math.Round(Position.y, 2) + ")," + "\n" +
                " size (" + tileWidth + "," + tileHeight + ")";
        }
    }
}
