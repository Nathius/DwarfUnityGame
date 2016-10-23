using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Econemy;

namespace Assets.Models.Buildings
{
    public enum BuildingType
    {
        RESOURCE_NODE,
        CITY_CENTER,
        WOOD_CUTTER,
        FARMER,
        MILL,
        COUNT
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
            return allDefinitions.Where(x => x.BuildingType == inBuildingType).FirstOrDefault();
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
            Conversion = new Conversion(2,
                new ResourceBundle(new ResourceAmmount(RESOURCE_TYPE.WOOD, 1)),
                new ConversionRequirement(new ResourceAmmount(RESOURCE_TYPE.WOOD, 1), true, 6)
                ),
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.WOOD, 10)
        };

        //farmer
        public static BuildingDefinition Farmer = new BuildingDefinition
        {
            BuildingType = BuildingType.FARMER,
            Conversion = new Conversion(5,
                new ResourceBundle(new ResourceAmmount(RESOURCE_TYPE.WHEAT, 2))
                ),
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.WOOD, 15)
        };

        //mill
        public static BuildingDefinition Mill = new BuildingDefinition
        {
            BuildingType = BuildingType.MILL,
            Conversion = new Conversion(8,
                new ResourceBundle(new ResourceAmmount(RESOURCE_TYPE.FLOUR, 6)),
                new ConversionRequirement(new ResourceAmmount(RESOURCE_TYPE.WHEAT, 32))
                ),
            BuildingCost = new ResourceAmmount(RESOURCE_TYPE.WOOD, 15)
        };
	}
}
