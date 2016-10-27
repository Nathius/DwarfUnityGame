using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;
using Assets.UnityWrappers;
using Assets.Models.Buildings;

namespace Assets.Units
{
	public class Unit : TeamEntity
	{
        public Vector2 TargetPosition { get; set; }
        public UnitType UnitType { get; set; }

        public int Health { get; set; }
        

        public Unit(UnityObjectWrapper viewObject, UnitType inUnitType)
            : base(viewObject)
        {
            UnitType = inUnitType;
        }

	}
}
