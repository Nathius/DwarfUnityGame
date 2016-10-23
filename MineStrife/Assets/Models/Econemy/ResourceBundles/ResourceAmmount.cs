using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Models.Econemy
{
	public class ResourceAmmount
	{
        public RESOURCE_TYPE ResourceType { get; set; }
        public int Ammount { get; set; }

        public ResourceAmmount(RESOURCE_TYPE inResourceType, int inAmmount)
        {
            ResourceType = inResourceType;
            Ammount = inAmmount;
        }

        public override string ToString()
        {
            return Ammount + " " + ResourceType.ToString(); 
        }

    }
}
