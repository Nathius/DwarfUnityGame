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

        public float CoolDownCounter { get; set; }

        public Weapon(WeaponDefinition inDefinition)
        {
            WeaponType = inDefinition.WeaponType;
            Range = inDefinition.Range;
            Damage = inDefinition.Damage;
            CoolDownTime = inDefinition.CoolDownTime;
            CoolDownCounter = 0;
        }

        public bool ReadyToFire()
        {
            return CoolDownCounter <= 0;
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
