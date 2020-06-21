using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Models.AI
{
    public enum CommandTypes
    {
        MOVE,
        FOLLOW,
        STOP,
        PATROLE,
        HOLD,
        ATTACK,
        ATTACK_MOVE,
        BUILD,
        CAST,
        WORK,


        _COUNT
    }
	public class Command
	{
        public CommandTypes CommandType { get; set; }

        public Vector2? TargetPosition { get; set; }
        public WorldEntity TargetEntity { get; set; }
        public bool Repeat { get; set; }

        public Command(CommandTypes inCommandType, Vector2? inTargetPosition, WorldEntity inTargetEntity, bool inRepeat )
        {
            CommandType = inCommandType;
            TargetPosition = inTargetPosition;
            TargetEntity = inTargetEntity;
            Repeat = inRepeat;
        }

	}
}
