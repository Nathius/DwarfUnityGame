using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.UnityWrappers;
using UnityEngine;

namespace Assets.Models
{
	public class TeamEntity : WorldEntity
	{
        private int team;

        public TeamEntity(UnityObjectWrapper viewObject, Vector2 inPosition, int inTeam)
            : base(viewObject, inPosition)
        {
            team = inTeam;
        }

        public int getTeam()
        {
            return team;
        }

	}
}
