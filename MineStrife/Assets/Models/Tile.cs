using System.Collections;
using System;
using UnityEngine;
using Assets.UnityWrappers;

namespace Assets.Models
{
    public enum TileType
    {
        NONE,
        DIRT,
        _COUNT
    }

    public class Tile : WorldEntity
    {
        public int Cost { get; set; }
        Action<Tile> TileTypeChangedCB;
        private TileType tileType = TileType.NONE;
        public TileType TileType
        {
            get
            {
                return tileType;
            }
            set
            {
                var oldType = tileType;
                tileType = value;
                //callback to update graphics
                if (oldType != tileType && TileTypeChangedCB != null)
                {
                    TileTypeChangedCB(this);
                }
            }
        }

        public Tile(UnityObjectWrapper viewObject, Vector2 inPosition)
            : base(viewObject, inPosition)
        {
            Position = inPosition;
        }

        public void RegisterTileTypeChangedCB(Action<Tile> inCallBack)
        {
            //registers multiple callabacks so many systems can hook into updates
            TileTypeChangedCB += inCallBack;
        }
        public void UnRegisterTileTypeChangedCB(Action<Tile> inCallBack)
        {
            //registers multiple callabacks so many systems can hook into updates
            TileTypeChangedCB -= inCallBack;
        }
    }
}

