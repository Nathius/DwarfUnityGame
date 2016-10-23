using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Econemy;

namespace Assets.Models.Buildings
{
	public class Conversion
	{

        public float TimeTaken { get; set; }

        public ResourceBundle Result { get; set; }

        public List<ConversionRequirement> Requirements { get; set; }

        public Conversion()
        {
            Requirements = new List<ConversionRequirement>();
        }

        public Conversion(float inTimeTaken, ResourceBundle inResult, List<ConversionRequirement> inRequirements)
        {
            TimeTaken = inTimeTaken;
            Result = inResult;
            Requirements = inRequirements;
        }

        public Conversion(float inTimeTaken, ResourceBundle inResult, ConversionRequirement inRequirement = null)
        {
            TimeTaken = inTimeTaken;
            Result = inResult;
            Requirements = new List<ConversionRequirement>();

            if(inRequirement != null)
            {
                Requirements.Add(inRequirement);
            }
        }

        public bool RequiresStockpileResources()
        {
            return StockpileRequirements().Any();
        }
        public List<ConversionRequirement> StockpileRequirements()
        {
            return Requirements.Where(x => x.requiresNode == false).ToList();
        }

        public bool RequiresNodeResources()
        {
            return NodeRequirements().Any();
        }
        public List<ConversionRequirement> NodeRequirements()
        {
            return Requirements.Where(x => x.requiresNode == true).ToList();
        }
	}
}
