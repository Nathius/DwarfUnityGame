using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Models.Econemy
{
	public class ResourceBundle
	{
        public List<ResourceAmmount> resources { get; set; }

        public ResourceBundle()
        {
            resources = new List<ResourceAmmount>();
        }

        public ResourceBundle(ResourceAmmount inResourceAmmount)
        {
            resources = new List<ResourceAmmount>();
            AddResource(inResourceAmmount);
        }

        public void AddResource(ResourceAmmount inResourceAmmount)
        {
            //add ammount to existing item or add item if not found in list
            if (resources.Where(x => x.ResourceType == inResourceAmmount.ResourceType).Any())
            {
                resources.Where(x => x.ResourceType == inResourceAmmount.ResourceType)
                    .First().Ammount += inResourceAmmount.Ammount;
            }
            else
            {
                resources.Add(inResourceAmmount);
            }
        }
	}
}
