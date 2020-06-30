﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Econemy;
using Assets.Models.AI;
using Assets.Models.Units;

namespace Assets.Models.Buildings
{
    public enum UnitType
    {
        NONE,
        WORKER,
        ARCHER,
        _COUNT
    }

    public class UnitDefinition
    {
        public List<ResourceAmmount> Cost { get; set; }
        public UnitType UnitType { get; set; }
        public List<CommandTypes> Behaviours { get; set; }
        public WeaponType Weapon { get; set; }

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
            Weapon = WeaponType.NONE,
            Cost = new List<ResourceAmmount>() {
                new ResourceAmmount(RESOURCE_TYPE.BREAD, 20)
            },
            Behaviours = new List<CommandTypes>() { CommandTypes.MOVE, CommandTypes.BUILD, CommandTypes.FOLLOW}
        };

        //Archer
        public static UnitDefinition Archer = new UnitDefinition
        {
            UnitType = UnitType.ARCHER,
            Weapon = WeaponType.SHORT_BOW,
            Cost = new List<ResourceAmmount>() { 
                new ResourceAmmount(RESOURCE_TYPE.BREAD, 20),
                new ResourceAmmount(RESOURCE_TYPE.WOOD, 20)
            },
            Behaviours = new List<CommandTypes>() { CommandTypes.MOVE, CommandTypes.FOLLOW, CommandTypes.ATTACK }
        };
    }
}
