using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using Assets.Models.AI.Routines.SubRoutine;
using UnityEngine;
using Assets.Models.Buildings;
using Assets.Models.Units;

namespace Assets.Models.AI.Routines
{
	public class BuildRoutine : Routine
	{
        private ProductionBuilding TargetBuilding { get; set; }

        private MoveToBuildingSubRoutine moveToBuilding { get; set; }
        private ConstructBuildingSubRoutine constructBuilding { get; set; }

        public BuildRoutine(Unit inBody, Command inCommand)
            : base(inBody, inCommand)
        {
            //negate command if target entity not provided
            if(inCommand.TargetEntity == null)
            {
                IsFinished = true;
                return;
            }


            if (inCommand.TargetEntity.GetType() != typeof(ProductionBuilding))
            {
                IsFinished = true;
                return;
            }

            TargetBuilding = (ProductionBuilding)inCommand.TargetEntity;

            moveToBuilding = new MoveToBuildingSubRoutine(inBody, TargetBuilding);
            constructBuilding = new ConstructBuildingSubRoutine(inBody, TargetBuilding);

            //movetoPoint = new MoveToPointSubRoutine(inBody, inCommand.TargetPosition.Value);
        }

        

        public override void Update(float inTimeDelta)
        {
            //exit if routine is finished, manager should pick up and delete routine
            if(IsFinished)
            {
                return;
            }

            Body.SetSpriteState(UnitSpriteState.BUILDING);

            //first move to building
            if (moveToBuilding.GetIsFinished() == false)
            {
                moveToBuilding.Update(inTimeDelta);
            }
            else if (moveToBuilding.GetIsFinished())
            {
                //TODO check we are still close enough to the building
                //var dist = TargetBuilding.Position - Body.Position;

                //then build the building
                if (constructBuilding.GetIsFinished() == false)
                {
                    constructBuilding.Update(inTimeDelta);
                }
                else
                {
                    IsFinished = true;
                    return;
                }
            }            
        }
	}
}
