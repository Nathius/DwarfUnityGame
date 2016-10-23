using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Models.Econemy
{
	public class Stockpile
	{
        private Dictionary<RESOURCE_TYPE, int> Stocks;
        private int MaxStockLevel { get; set; }

        public Stockpile()
        {
            MaxStockLevel = 500;
            Stocks = new Dictionary<RESOURCE_TYPE, int>();
            for (int i = 0; i < (int)RESOURCE_TYPE.COUNT; i++ )
            {
                Stocks.Add((RESOURCE_TYPE)i, 0);
            }
        }

        private void ClampStock(RESOURCE_TYPE inStockType)
        {
            if (Stocks[inStockType] < 0)
            {
                Stocks[inStockType] = 0;
            }
            else if (Stocks[inStockType] > MaxStockLevel)
            {
                Stocks[inStockType] = MaxStockLevel;
            }
        }

        public int GetStockLevel(RESOURCE_TYPE inStockType)
        {
            if (Stocks.ContainsKey(inStockType))
            {
                return Stocks[inStockType];
            }
            //"Error, requested stock level for non existant stock: " + inStockType
            return 0;
        }

        public void AddStock(RESOURCE_TYPE inStockType, int inAmmount)
        {
            if (Stocks.ContainsKey(inStockType))
            {
                Stocks[inStockType] += Math.Abs(inAmmount);
            }

            ClampStock(inStockType);
        }
        public void AddStock(ResourceAmmount inResourceAmmount)
        {
            AddStock(inResourceAmmount.ResourceType, inResourceAmmount.Ammount);
        }
        public void AddStock(ResourceBundle inResourceBundle)
        {
            foreach(var res in inResourceBundle.resources)
            {
                AddStock(res);
            }
            
        }

        public void RemoveStock(RESOURCE_TYPE inStockType, int inAmmount)
        {
            if (Stocks.ContainsKey(inStockType))
            {
                Stocks[inStockType] -= Math.Abs(inAmmount);
            }

            ClampStock(inStockType);
        }
        public void RemoveStock(ResourceAmmount inResourceAmmount)
        {
            RemoveStock(inResourceAmmount.ResourceType, inResourceAmmount.Ammount);
        }

        public bool CanAfford(ResourceAmmount inCost)
        {
            if(Stocks[inCost.ResourceType] >= inCost.Ammount)
            {
                return true;
            }
            return false;
        }

	}
}
