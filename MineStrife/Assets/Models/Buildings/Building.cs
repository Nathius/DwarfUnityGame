using System.Collections;
using System;
using Assets.UnityWrappers;
using Assets.Models.Buildings;

namespace Assets.Models
{

    public class Building : TeamEntity
    {
        public int tileWidth { get; set; }
        public int tileHeight { get; set; }
        public BuildingType BuildingType { get; set; }

        public Building(UnityObjectWrapper viewObject,
            int inWidth, 
            int inHeight, 
            BuildingType inBuildingType)
            : base(viewObject)
        {
            tileWidth = inWidth;
            tileHeight = inHeight;
            BuildingType = inBuildingType;
        }

        public override string ToString()
        {
            return "Building " + BuildingType.ToString() + "\n" +
                " (" + Math.Round(Position.x, 2) + "," + Math.Round(Position.y, 2) + ")," + "\n" +
                " size (" + tileWidth + "," + tileHeight + ")";
        }
    }
}
