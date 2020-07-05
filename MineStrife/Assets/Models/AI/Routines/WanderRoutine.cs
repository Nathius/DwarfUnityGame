using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;
using Assets.Models.AI.Routines.SubRoutine;
using Assets.Models.Units;
using UnityEngine;
using Assets.Scripts;

namespace Assets.Models.AI.Routines
{
    public class WanderRoutine : Routine
	{
        private MoveToPointSubRoutine movetoPoint { get; set; }

        private const float MaxPauseTime = 10;
        private const float MinPauseTime = 2;
        private float PauseCounter { get; set; }
        private float TargetPauseTime { get; set; }


        public WanderRoutine(Unit inBody, Command inCommand)
            : base(inBody, inCommand)
        {
            PauseCounter = 0;
            TargetPauseTime = 0;

            var randomPos = NewRandomPosition();
            if (randomPos.HasValue)
            {
                movetoPoint = new MoveToPointSubRoutine(inBody, randomPos.Value);
            }   
        }

        public Vector2? NewRandomPosition()
        {
            for (var i = 0; i < ConfigFlags.WanderTryCount; i++ )
            {
                int newX = (int) Body.Position.x + UnityEngine.Random.Range((-ConfigFlags.WanderDist), ConfigFlags.WanderDist);
                int newY = (int) Body.Position.y + UnityEngine.Random.Range((-ConfigFlags.WanderDist), ConfigFlags.WanderDist);

                //check pos in world grid
                if (World.Instance.PointInGrid(newX, newY) == false)
                {
                    continue;
                }

                var newPos = new Vector2(newX, newY);
                var freePos = GridHelper.GetTilePositionIfFree(newPos);
                if (freePos.HasValue)
                {
                    return freePos;
                }

            }
            return null;
        }
        

        public override void Update(float inTimeDelta)
        {
            Body.SetSpriteState(UnitSpriteState.MOVING);


            //if moving
            if (movetoPoint != null && movetoPoint.GetIsFinished() == false)
            {
                //continue moving
                movetoPoint.Update(inTimeDelta);
            }
            else //if finished moving
            {
                movetoPoint = null;
                //if timer at zero
                if(TargetPauseTime == 0)
                {
                    //set new random time
                    TargetPauseTime = UnityEngine.Random.Range(MinPauseTime, MaxPauseTime);
                }
                else if(PauseCounter <= TargetPauseTime) //if current time < target time
                {
                    //idle
                    //incrament time
                    PauseCounter += inTimeDelta;
                }
                else //if finished
                {
                    //new move location
                    TargetPauseTime = 0;
                    PauseCounter = 0;

                    var randomPos = NewRandomPosition();
                    if (randomPos.HasValue)
                    {
                        movetoPoint = new MoveToPointSubRoutine(Body, randomPos.Value);
                    }
                    //reset timer to 0
                    
                }
            }
        }
	}
}
