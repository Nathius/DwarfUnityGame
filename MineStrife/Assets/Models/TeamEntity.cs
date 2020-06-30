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
        protected int Team {get; set; }
        protected int MaxHealth { get; set; }
        protected int Health {get; set; }
        protected bool IsDead { get; set; }

        public TeamEntity(UnityObjectWrapper viewObject, Vector2 inPosition, int inTeam)
            : base(viewObject, inPosition)
        {
            Team = inTeam;
            IsDead = false;
        }

        public bool GetIsDead()
        {
            return IsDead;
        }

        public int getTeam()
        {
            return Team;
        }

        public virtual void Die()
        {
            Health = 0;
            IsDead = true;
        }

        public void TakeDamage(int inDamage)
        {
            Health -= inDamage;

            if (Health <= 0)
            {
                this.Die();
            }
        }
	}
}
