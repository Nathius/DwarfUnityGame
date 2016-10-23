using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Models.Econemy
{
	public class ConversionRequirement
	{
        public ResourceAmmount resourceAmmount { get; set; }
        public bool requiresNode { get; set; }
        public float harvestDistance { get; set; }

        public ConversionRequirement(ResourceAmmount inResourceAmmount, bool inRequiresNode = false, float inHarvestDistance = 0)
        {
            resourceAmmount = inResourceAmmount;
            requiresNode = inRequiresNode;
            harvestDistance = inHarvestDistance;
        }
	}
}
