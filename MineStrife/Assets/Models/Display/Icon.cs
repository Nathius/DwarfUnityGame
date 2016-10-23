using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models.Buildings;

namespace Assets.Models.Display
{
	public class Icon
	{
        public GameObject gameObject;
        public BuildingType buildingType;

        public Icon (BuildingType inBuildingType)
        {
            buildingType = inBuildingType;
        }

	}

}
