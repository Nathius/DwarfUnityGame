using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models.Buildings;

namespace Assets.Controllers.PrefabControllers
{
    public class BuildingPrefabController
	{
        public static PrefabAssetController<BuildingType> Instance { get; set;  }

        public static Dictionary<BuildingType, string> GetPaths()
        {
            var paths = new Dictionary<BuildingType, string>();
            string basePath = "Buildings/";
            paths.Add(BuildingType.CITY_CENTER, basePath + "Building_Stronghold");
            paths.Add(BuildingType.SITE_3X3, basePath + "Building_Site_3x3");
            paths.Add(BuildingType.WOOD_CUTTER, basePath + "Building_WoodCutter");
            paths.Add(BuildingType.FARMER, basePath + "Building_Farmer");
            paths.Add(BuildingType.MILL, basePath + "Building_Mill");
            paths.Add(BuildingType.BARRACKS, basePath + "Building_Barracks");

            return paths;
        }

	}
}
