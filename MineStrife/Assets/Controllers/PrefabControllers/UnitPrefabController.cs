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
        public static PrefabAssetController<UnitTypes> Instance { get; set;  }

        public static Dictionary<UnitTypes, string> GetPaths()
        {
            var paths = new Dictionary<UnitTypes, string>();
            string basePath = "Units/";
            paths.Add(UnitTypes.WORKER, basePath + "Unit_Worker");

            return paths;
        }

	}
}
