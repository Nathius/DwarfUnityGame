using System.Collections;
using System.Collections.Generic;
using Assets.Models.Econemy;
using UnityEngine;
using Assets.Scripts;
using Assets.Controllers;
using Assets.Models.Buildings;

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
        public Vector2 Size;

        private bool haveSpawnedStartingUnits = false;

        public World(int inWidth = 10, int inHeight = 10)
        {
            Instance = this;
            PopCap = 10;
            CurrentPop = 0;

            //new empty list of game objects
            all_worldEntity = new List<WorldEntity>();
            _stockpile = new Stockpile();

            //put some default resources into the stockpile
            _stockpile.AddStock(RESOURCE_TYPE.WOOD, 60);
            _stockpile.AddStock(RESOURCE_TYPE.STONE, 60);
            _stockpile.AddStock(RESOURCE_TYPE.WHEAT, 30);
            _stockpile.AddStock(RESOURCE_TYPE.BREAD, 60);

            Size = new Vector2(inWidth, inHeight);
            tiles = new Tile[inWidth, inHeight];
        }

        private void SpawnStartingUnits(int number)
        {
            var centerPos = new Vector2(World.Instance.GetWidth() * 0.5f, World.Instance.GetHeight() * 0.5f);
            for (int i = 0; i < number; i++)
            {
                World.Instance.CurrentPop++;
                var pos = new Vector2(centerPos.x + UnityEngine.Random.Range(-4, 4), centerPos.y + UnityEngine.Random.Range(-4, 4));
                UnitController.Instance.CreateUnitAt(pos, UnitType.WORKER, WorldController.PlayerTeam);
            }
        }

        public int GetWidth()
        {
            return (int)Size.x;
        }
        public int GetHeight()
        {
            return (int)Size.y;
        }

        public void Update(float inTimeDelta)
        {
            for (var i = 0; i < all_worldEntity.Count; i++)
            {
                all_worldEntity[i].Update(inTimeDelta);
            }

            //need to find a better way of initial start up when nothing is ready predictably
            if(!haveSpawnedStartingUnits)
            {
                SpawnStartingUnits(7);
                haveSpawnedStartingUnits = true;         
            }
        }

        public Tile GetTileAt(Vector2 inPosition)
        {
            return GetTileAt((int)inPosition.x, (int)inPosition.y);
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
            if (inX >= GetWidth() || inX < 0 || inY >= GetHeight() || inY < 0)
            {
                return false;
            }
            return true;
        }

        public WorldEntity EntityAtPosition(Vector3 inPosition)
        {
            //collision checking against all entities to see if the point is contained in any bounding box
            foreach (var entity in all_worldEntity)
            {
                BoxCollider2D boxCollider = entity.ViewObject.GetUnityGameObject().GetComponent<BoxCollider2D>();

                PolygonCollider2D polygonCollider = entity.ViewObject.GetUnityGameObject().GetComponent<PolygonCollider2D>();

                var boxColliderContainsPosition = boxCollider != null 
                    && boxCollider.enabled 
                    && boxCollider.bounds.Contains(new Vector3(inPosition.x, inPosition.y, boxCollider.bounds.center.z));

                //polygon collider apparently works using a square bounding box =/
                //the fucck is the point of a polygon collider then??

                var polygonColliderContainsPosition = polygonCollider != null 
                    && polygonCollider.enabled 
                    && polygonCollider.bounds.Contains(new Vector3(inPosition.x, inPosition.y, polygonCollider.bounds.center.z));

                if ((boxColliderContainsPosition 
                        || polygonColliderContainsPosition) 
                    && entity.GetType() != typeof(Tile))
                {
                    return entity;
                }
            }

            return null;
        }

        public List<WorldEntity> EntitiesNearPosition(Vector2 inPosition, float inDistance)
        {
            List<WorldEntity> nearbyEntities = new List<WorldEntity>();
            foreach (var entity in all_worldEntity)
            {
                if (VectorHelper.getDistanceBetween(inPosition, entity.Position) <= inDistance)
                {
                    nearbyEntities.Add(entity);
                }
            }
            return nearbyEntities;
        }
    }
}