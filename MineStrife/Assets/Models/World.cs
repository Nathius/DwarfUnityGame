using System.Collections;
using System.Collections.Generic;
using Assets.Models.Econemy;
using UnityEngine;

namespace Assets.Models
{
    public class World
    {
        public static World Instance;
        private Stockpile _stockpile;
        public Stockpile Stockpile { get { return _stockpile; } }
        public int PopCap { get; set; }
        public int CurrentPop { get; set; }

        public Building CityCenter { get; set; }

        public Tile[,] tiles;
        public static List<WorldEntity> all_worldEntity;
        int width;
        int height;

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public World(int inWidth = 10, int inHeight = 10)
        {
            Instance = this;
            PopCap = 4;
            CurrentPop = 0;

            //new empty list of game objects
            all_worldEntity = new List<WorldEntity>();
            _stockpile = new Stockpile();

            //put some default resources into the stockpile
            _stockpile.AddStock(RESOURCE_TYPE.WOOD, 60);
            _stockpile.AddStock(RESOURCE_TYPE.STONE, 60);
            _stockpile.AddStock(RESOURCE_TYPE.WHEAT, 30);
            _stockpile.AddStock(RESOURCE_TYPE.BREAD, 60);

            width = inWidth;
            height = inHeight;

            tiles = new Tile[width, height];
        }

        public void Update(float inTimeDelta)
        {
            for (var i = 0; i < all_worldEntity.Count; i++ )
            {
                all_worldEntity[i].Update(inTimeDelta);
            }
        }

        public Tile GetTileAt(int inX, int inY)
        {
            if (PointInGrid(inX, inY))
            {
                return tiles[inX, inY];
            }
            return null;
        }
        public void SetTileAt(int inX, int inY, Tile inTile)
        {
            if (PointInGrid(inX, inY))
            {
                tiles[inX, inY] = inTile;
            }
        }
        private bool PointInGrid(int inX, int inY)
        {
            if (inX >= width || inX < 0 || inY >= height || inY < 0)
            {
                return false;
            }
            return true;
        }

        public WorldEntity EntityAtPosition(Vector3 inPosition)
        {
            //collision checking against all entities to see if the point is contained in any bounding box
            foreach(var entity in all_worldEntity)
            {
                var collider = entity.ViewObject.GetUnityGameObject().GetComponent<BoxCollider2D>();
                if (collider != null && collider.bounds.Contains(inPosition))
                {
                    return entity;
                }
            }

            return null;
        }

    }
}