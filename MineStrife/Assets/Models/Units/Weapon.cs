using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Units;

namespace Assets.Models.Units
{
	public class Weapon
	{
        public WeaponType WeaponType { get; set; }
        public float Range { get; set; }
        public int Damage { get; set; }
        public float CoolDownTime { get; set; }
        public float AimTime { get; set; }

        public float CoolDownCounter { get; set; }

        public Weapon(WeaponDefinition inDef)
        {
            WeaponType = inDef.WeaponType;
            Range = inDef.Range;
            Damage = inDef.Damage;
            CoolDownTime = inDef.CoolDownTime;
            AimTime = inDef.AimTime;
            CoolDownCounter = 0;
        }

        public bool ReadyToFire()
        {
            return CoolDownCounter <= 0;
        }

        public bool Aiming()
        {
            if (CoolDownCounter > 0 && CoolDownCounter < AimTime)
            {
                return true;
            }
            return false;
        }

        public void FireAtUnit(TeamEntity inTarget)
        {
            //assumes checks for in range and such are handeled elsewhere
            inTarget.TakeDamage(Damage);
            CoolDownCounter = CoolDownTime;
        }

        public void Update(float inTimeDelta)
        {
            if(CoolDownCounter > 0)
            {
                CoolDownCounter -= inTimeDelta;
            }
        }
	}
}
