using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Models.Econemy;
using Assets.Models.Buildings;
using UnityEngine;
using Assets.UnityWrappers;
using Assets.Models.Econemy.ResourceNodes;
using Assets.Controllers;

namespace Assets.Models.Buildings
{
	public class ProductionBuilding : Building
	{
        //passive productions
        public float TimeRemaining { get; set; }

        public Conversion Conversion { get; set; }
        public bool currentlyProcessing { get; set; }

        public ProductionBuilding(UnityObjectWrapper viewObject, int inWidth, int inHeignt, BuildingType inBuildingType, Conversion inConversion)
            : base(viewObject, inWidth, inHeignt, inBuildingType)
        {
            currentlyProcessing = false;
            //automatically start their first conversion
            if(inConversion != null)
            {
                Conversion = inConversion;
                TimeRemaining = Conversion.TimeTaken;
            }
        }

        public void CheckCanStartProcessing()
        {
            //should only be checked if not processing
            if (Conversion != null)
            {
                bool canAfford_fromStockpile = true;
                bool canAfford_fromNodes = true;
                var foundNodes = new Dictionary<RESOURCE_TYPE, ResourceNode>();

                //check any stockpile requirements
                if (Conversion.RequiresStockpileResources())
                {
                    var requiredStock = Conversion.StockpileRequirements();
                    for (int r = 0; r < requiredStock.Count() && canAfford_fromStockpile; r++ )
                    {
                        //check if the stockpile can support all resource requirements
                        if ( ! World.Instance.Stockpile.CanAfford(requiredStock[r].resourceAmmount))
                        {
                            canAfford_fromStockpile = false;
                        }
                    }
                }

                //check any node requirements
                if (canAfford_fromStockpile && Conversion.RequiresNodeResources())
                {
                    var requiredNodes = Conversion.NodeRequirements();

                    //check that we can get the required resources from every node
                    for (int n = 0; n < requiredNodes.Count() && canAfford_fromNodes; n++)
                    {
                        //get resource node within range
                        var node = ResourceNode.ClosestNodeToPoint(Position, requiredNodes[n].harvestDistance, requiredNodes[n].resourceAmmount.ResourceType);
                        //check if the node can support the required harvest ammount
                        if (node != null && node.CanExtractResource(requiredNodes[n].resourceAmmount.Ammount))
                        {
                            foundNodes.Add(requiredNodes[n].resourceAmmount.ResourceType, node);
                        }
                        else
                        {
                            canAfford_fromNodes = false;
                        }
                    }
                }
                
                //if can afford process start prcessing
                if (canAfford_fromStockpile && canAfford_fromNodes)
                {
                    StartConversion(foundNodes);
                }
            }

        }

        private void StartConversion(Dictionary<RESOURCE_TYPE, ResourceNode> inFoundNodes)
        {
            //extract all required resources from the stockpile
            var requiredResources = Conversion.StockpileRequirements();
            foreach (var stockReq in requiredResources)
            {
                World.Instance.Stockpile.RemoveStock(stockReq.resourceAmmount);
            }
            
            //extract all required resources from nodes
            var requiredNodes = Conversion.NodeRequirements();
            foreach(var nodeReq in requiredNodes)
            {
                //grab the found node for that resource and harvest it
                var node = inFoundNodes[nodeReq.resourceAmmount.ResourceType];
                node.extractResource(nodeReq.resourceAmmount.Ammount);
            }


            //start the process timer
            currentlyProcessing = true;
            TimeRemaining = Conversion.TimeTaken;

        }

        public override void Update(float inTimeDelta)
        {
            base.Update(inTimeDelta);

            if (currentlyProcessing)
            {
                CheckFinishConversion(inTimeDelta);
            }
            else
            {
                CheckCanStartProcessing();
            }
        }

        private void CheckFinishConversion(float inTimeDelta)
        {
            TimeRemaining -= inTimeDelta;

            if (TimeRemaining <= 0)
            {
                currentlyProcessing = false;

                //action any resource bundle results of the conversion
                if (Conversion.Result.ResourceBundle != null)
                {
                    World.Instance.Stockpile.AddStock(Conversion.Result.ResourceBundle);
                    DisplayController.Instance.CreateProductionNumber(Position, Conversion.Result.ResourceBundle);
                }
                if(Conversion.Result.UnitType != null)
                {
                    UnitController.Instance.CreateUnitAt(Position, Conversion.Result.UnitType.Value);
                    DisplayController.Instance.CreateProductionLable(Position, "+1 " + Conversion.Result.UnitType.ToString());
                }
                
            }
        }

        public override string ToString()
        {
            var displayStr = "Production building " + BuildingType.ToString() + "\n";
            if (Conversion != null)
            {
                displayStr +=  Conversion.Description + ": " + ProductionBar() + "\n";
            }

            displayStr += " (" + Math.Round(Position.x, 2) + "," + Math.Round(Position.y, 2) + ")," + "\n";
            displayStr += " size (" + tileWidth + "," + tileHeight + ")";

            return displayStr;
        }

        public string ProductionBar()
        {
            var bar = "[";
            if(currentlyProcessing)
            {
                var progress = (Conversion.TimeTaken - TimeRemaining) / Conversion.TimeTaken;
                var pips = (int)Math.Round(progress * 10);
                for (int i = 0; i <= pips; i++)
                {
                    bar += "+";
                }
                for (int i = 0; i < (10 - pips); i++)
                {
                    bar += "-";
                }
            }
            
            bar += "]";
            return bar;

        }
	}
}
