using System.Collections;
using System;
using Assets.UnityWrappers;
using Assets.Models.Buildings;
using UnityEngine;
using Assets.Scripts;
using Assets.Controllers.PrefabControllers;
using Assets.Controllers;

namespace Assets.Models
{

    public class Building : TeamEntity
    {
        public static readonly int MaxConstructionProgress = 1000;
        private static readonly Vector2 DefaultSize = new Vector2(2, 2);
        public BuildingType BuildingType { get; set; }
        public Vector2 Size { get; set; }

        public bool IsUnderConstruction { get; set; }
        public int ConstructionProgress { get; set; }

        public Building(UnityObjectWrapper viewObject,
            Vector2 inPosition,
            BuildingType inBuildingType)
            : base(viewObject, inPosition)
        {
            BuildingType = inBuildingType;
            SetBuildingSize();
            
            //viewObject.SetBoundSize(Size);
            ViewObject.AddOrUpdateGridBaseCollider(inPosition, Size);
            GridHelper.AddBuildingToCollisionMap(inPosition, Size);
        }

        //update if under construction
        public override void Update(float inTimeDelta)
        {
            //temporary, buildings build themselves
            if (IsUnderConstruction)
            {
                constructBuilding(1);
            }

            base.Update(inTimeDelta);
        }

        //adds progress to building construction
        // potential for repair function
        // returns true when progress accepted ; and false when finished
        public bool constructBuilding(int progress)
        {
            if (!IsUnderConstruction)
            {
                return false; //is already finished
                // change to repair hp off damaged building??
            }

            ConstructionProgress += progress;

            if (ConstructionProgress >= MaxConstructionProgress)
            {
                FinishConstruction();
                return false;
            }

            return true;
        }

        public void FinishConstruction()
        {
            IsUnderConstruction = false;

            //delete old view object
            var oldGO = ViewObject;
            GameObject.Destroy(oldGO.GetUnityGameObject());
            
            //swap with finished sprite
            var newPrefab = BuildingPrefabController.Instance.GetPrefab(BuildingType);
            GameObject building_go = UnityEngine.Object.Instantiate(newPrefab);
            building_go.transform.SetParent(BuildingController.Instance.transform, true);
            building_go.name = "building";

            //update objects view object and set colliders
            this.ViewObject = new UnityObjectWrapper(building_go);
            ViewObject.AddOrUpdateGridBaseCollider(Position, Size);
            GridHelper.AddBuildingToCollisionMap(Position, Size);
            
        }

        public Vector2 ReferencePosition()
        {
            return GridHelper.BuildingCenter(Position, Size);
        }

        private void SetBuildingSize()
        {
            var definition = BuildingDefinition.GetBuildingDefinitionForType(BuildingType);
            if (definition != null)
            {
                Size = definition.Size;
            }
            else
            {
                Size = DefaultSize;
            }
        }

        public override string ToString()
        {
            var str = "Building " + BuildingType.ToString() + "\n";
            
            if (this.IsUnderConstruction)
            {


                str += "Under construction : " + ProgressBarHelper.GetBar(ConstructionProgress, Building.MaxConstructionProgress) + "\n";
            }

            str += " Position: " + VectorHelper.ToString(Position) + "\n";
            str += " Size: " + VectorHelper.ToString(Size) + "\n";

            return str;
        }
    }
}
