using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models;
using Assets.Models.Buildings;
using Assets.UnityWrappers;
using Assets.Models.Econemy.ResourceNodes;
using Assets.Controllers.PrefabControllers;
using Assets.Units;
using Assets.Models.AI;
using Assets.Scripts;
using Assets.Models.Units;

namespace Assets.Controllers
{
    public class UnitController : MonoBehaviour
	{
        public static UnitController Instance { get; protected set; }

        void Start()
        {
            if (Instance != null)
            { 
                Debug.LogError(this.GetType().ToString() + " already instanced");
            }
            Instance = this;

        }

        public void CreateUnitAt(Vector2 inPos, UnitType inUnitType, int team)
        {
            var spawnPos = GridHelper.GetClosestFreePosition(inPos);

            if (spawnPos != null)
            {
                var prefab = UnitPrefabController.Instance.GetPrefab(inUnitType);
                var unitDef = UnitDefinition.GetDefinitionForType(inUnitType);
                Weapon weapon = null;

                if (unitDef.Weapon != WeaponType.NONE)
                {
                    var wepDef = WeaponDefinition.GetDefinitionForType(unitDef.Weapon);
                    weapon = new Weapon(wepDef);
                }

                //instance a new build prefab and place it on the screen
                GameObject unit_go = Instantiate(prefab);
                unit_go.transform.SetParent(this.transform, true);
                unit_go.name = "Unit_" + inUnitType.ToString();

                //create a new unit
                var ai = new AI(unitDef.Behaviours);
                new Unit(new UnityObjectWrapper(unit_go), spawnPos.Value, team, inUnitType, ai, weapon);
            }         
        }

        

        

        
	}
}
