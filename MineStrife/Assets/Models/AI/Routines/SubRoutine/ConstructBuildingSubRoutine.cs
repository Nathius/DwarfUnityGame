using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Units;
using Assets.Models.AI.PathFinding;
using Assets.Scripts;
using Assets.Models.Buildings;

namespace Assets.Models.AI.Routines.SubRoutine
{
	public class ConstructBuildingSubRoutine
	{
        private Unit Body { get; set; }
        private ProductionBuilding TargetBuilding { get; set; }

        private bool isFinished;

        public ConstructBuildingSubRoutine(Unit inBody, ProductionBuilding inBuilding)
        {
            TargetBuilding = inBuilding;
            Body = inBody;
            isFinished = false;

            if (inBuilding == null)
            {
                isFinished = true;
                return;
            }
        }

        public bool GetIsFinished()
        {
            return isFinished;
        }

        public void Update(float inTimeDelta)
        {
            if (isFinished)
            {
                //managing routine should pick up the finish and delete the sub routine
                return; 
            }

            //check building still exists
            if(TargetBuilding == null)
            {
                isFinished = true;
                return;
            }

            //check building is not finished
            if (TargetBuilding.IsUnderConstruction == false)
            {
                isFinished = true;
                return;
            }

            Construct();
        }

        private void Construct()
        {
            //show building sprite ?

            TargetBuilding.constructBuilding(1);
        }

       
	}
}
