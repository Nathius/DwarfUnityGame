using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Econemy;
using UnityEngine;

namespace Assets.Models.Buildings
{
    public enum BuildingType
    {
        RESOURCE_NODE,
        CITY_CENTER,
        WOOD_CUTTER,
        FARMER,
        MILL,
        BARRACKS,
        _COUNT
    }

	public class BuildingDefinition
	{
        public Conversion Conversion{ get; set; }
        public ResourceAmmount BuildingCost { get; set; }
        public BuildingType BuildingType { get; set; }

        public static List<BuildingDefinition> allDefinitions { get; set; }
        public BuildingDefinition()
        {
            if (allDefinitions == null)
            {
                allDefinitions = new List<BuildingDefinition>();
            }
            allDefinitions.Add(this);
        }

        public static BuildingDefinition GetBuildingDefinitionForType(BuildingType inBuildingType)
        {
            var definition = allDefinitions.Where(x => x.BuildingType == inBuildingType).FirstOrDefault();
            if (definition == null)
            {
                Debug.Log("No building definition found for type: " + inBuildingType.ToString());
            }
            return definition;
        }

        //city center
        public static BuildingDefinition CityCenter = new BuildingDefinition
        {
            BuildingType = BuildingType.CITY_CENTER,
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.STONE, 20)
        };

        //woodcutter
        public static BuildingDefinition WoodCutter = new BuildingDefinition
        {
            BuildingType = BuildingType.WOOD_CUTTER,
            Conversion = new Conversion("Cut Wood", 2,
                new ConversionResult(
                    new ResourceBundle(new ResourceAmmount(RESOURCE_TYPE.WOOD, 1)),
                    null),
                new ConversionRequirement(new ResourceAmmount(RESOURCE_TYPE.WOOD, 1), true, 6)
                ),
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.WOOD, 10)
        };

        //farmer
        public static BuildingDefinition Farmer = new BuildingDefinition
        {
            BuildingType = BuildingType.FARMER,
            Conversion = new Conversion("Harvest Wheat", 5,
                new ConversionResult(
                    new ResourceBundle(new ResourceAmmount(RESOURCE_TYPE.WHEAT, 2)),
                    null)
                ),
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.WOOD, 15)
        };

        //mill
        public static BuildingDefinition Mill = new BuildingDefinition
        {
            BuildingType = BuildingType.MILL,
            Conversion = new Conversion("Mill Flour", 8,
                new ConversionResult(
                    new ResourceBundle(new ResourceAmmount(RESOURCE_TYPE.BREAD, 6)),
                    null),
                new ConversionRequirement(new ResourceAmmount(RESOURCE_TYPE.WHEAT, 15))
                ),
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.WOOD, 15)
        };

        //Barracks
        public static BuildingDefinition Barracks = new BuildingDefinition
        {
            BuildingType = BuildingType.BARRACKS,
            Conversion = new Conversion("Train Worker", 1,
                new ConversionResult(
                    null,
                    UnitType.WORKER),
                new ConversionRequirement(new ResourceAmmount(RESOURCE_TYPE.BREAD, 10))
                ),
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.WOOD, 20)
        };
	}
}
