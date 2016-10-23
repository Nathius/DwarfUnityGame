using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Econemy;

namespace Assets.Models.Buildings
{
    public enum UnitTypes
    {
        WORKER,
        COUNT
    }

    //public class UnitDefinition
    //{
    //    public Conversion Conversion{ get; set; }
    //    public ResourceAmmount BuildingCost { get; set; }
    //    public BuildingType BuildingType { get; set; }

    //    public static List<BuildingDefinition> allDefinitions { get; set; }
    //    public BuildingDefinition()
    //    {
    //        if (allDefinitions == null)
    //        {
    //            allDefinitions = new List<BuildingDefinition>();
    //        }
    //        allDefinitions.Add(this);
    //    }

    //    public static BuildingDefinition GetBuildingDefinitionForType(BuildingType inBuildingType)
    //    {
    //        return allDefinitions.Where(x => x.BuildingType == inBuildingType).FirstOrDefault();
    //    }

    //    //city center
    //    public static BuildingDefinition CityCenter = new BuildingDefinition
    //    {
    //        BuildingType = BuildingType.CITY_CENTER,
    //        BuildingCost = new ResourceAmmount(RESOURCE_TYPE.STONE, 20)
    //    };
    //}
}
