using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Buildings;

namespace Assets.Models.Econemy
{
	public class ConversionResult
	{
        public ResourceBundle ResourceBundle { get; set; }
        public UnitType? UnitType {get; set; }

        public ConversionResult(ResourceBundle inResourceBundle, UnitType? inUnitType)
        {
            ResourceBundle = inResourceBundle;
            UnitType = inUnitType;
        }
	}
}
