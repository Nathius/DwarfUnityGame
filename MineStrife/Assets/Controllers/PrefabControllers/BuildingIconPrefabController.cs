using UnityEngine;
using System.Collections;
using Assets.Models.Buildings;
using System.Collections.Generic;

namespace Assets.Controllers.PrefabControllers
{
    public class BuildingIconPrefabController
    {
        public static PrefabAssetController<BuildingType> Instance { get; set; }

        public static Dictionary<BuildingType, string> GetPaths()
        {
            var paths = new Dictionary<BuildingType, string>();
            string basePath = "BuildingIcons/";
            paths.Add(BuildingType.CITY_CENTER, basePath + "Icon_CityCenter");
            paths.Add(BuildingType.WOOD_CUTTER, basePath + "Icon_WoodCutter");
            paths.Add(BuildingType.FARMER, basePath + "Icon_Farmer");
            paths.Add(BuildingType.MILL, basePath + "Icon_Mill");
            paths.Add(BuildingType.BARRACKS, basePath + "Icon_Barracks");

            return paths;
        }
        
    }
}