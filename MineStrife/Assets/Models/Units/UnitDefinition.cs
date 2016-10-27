using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Econemy;

namespace Assets.Models.Buildings
{
    public enum UnitType
    {
        WORKER,
        COUNT
    }

    public class UnitDefinition
    {
        public int Health{ get; set; }
        public ResourceAmmount Cost { get; set; }
        public UnitType UnitType { get; set; }

        public static List<UnitDefinition> allDefinitions { get; set; }
        public UnitDefinition()
        {
            if (allDefinitions == null)
            {
                allDefinitions = new List<UnitDefinition>();
            }
            allDefinitions.Add(this);
        }

        public static UnitDefinition GetDefinitionForType(UnitType inType)
        {
            return allDefinitions.Where(x => x.UnitType == inType).FirstOrDefault();
        }

        //Worker
        public static UnitDefinition Worker = new UnitDefinition
        {
            UnitType = UnitType.WORKER,
            Cost = new ResourceAmmount(RESOURCE_TYPE.BREAD, 20)
        };
    }
}
