using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models.Buildings;

namespace Assets.Controllers.PrefabControllers
{
    public class UnitPrefabController
	{
        public static PrefabAssetController<UnitType> Instance { get; set;  }

        public static Dictionary<UnitType, string> GetPaths()
        {
            var paths = new Dictionary<UnitType, string>();
            string basePath = "Units/";
            paths.Add(UnitType.WORKER, basePath + "Unit_Worker");

            return paths;
        }

	}
}
