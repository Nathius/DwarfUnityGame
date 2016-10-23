using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.UnityWrappers;

namespace Assets.Models
{
	public class TeamEntity : WorldEntity
	{
        private int team;

        public TeamEntity(UnityObjectWrapper viewObject)
            : base(viewObject)
        {
        }

	}
}
