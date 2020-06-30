using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Models.Units
{

    public enum WeaponType
    {
        NONE,
        SHORT_BOW,
        LONG_BOW,
        CROSS_BOW,
        SPEAR,
        SWORD,
        AXE,
        _COUNT
    }

	public class WeaponDefinition
	{
        public WeaponType WeaponType { get; set; }
        public float Range {get; set; }
        public int Damage {get; set; }
        public float CoolDownTime {get; set; }

        public static List<WeaponDefinition> allDefinitions { get; set; }
        public WeaponDefinition()
        {
            if (allDefinitions == null)
            {
                allDefinitions = new List<WeaponDefinition>();
            }
            allDefinitions.Add(this);
        }

        public static WeaponDefinition GetDefinitionForType(WeaponType inType)
        {
            return allDefinitions.Where(x => x.WeaponType == inType).FirstOrDefault();
        }

        public static WeaponDefinition ShortBow = new WeaponDefinition
        {
            WeaponType = WeaponType.SHORT_BOW,
            CoolDownTime = 3,
            Damage = 1,
            Range = 6
        };
	}
}
