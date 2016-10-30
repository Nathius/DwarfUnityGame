using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Models;
using Assets.Models.Econemy.ResourceNodes;
using Assets.UnityWrappers;
using Assets.Models.Econemy;
using Assets.Controllers.PrefabControllers;
using Assets.Models.Buildings;

namespace Assets.Controllers
{
    public class WorldController : MonoBehaviour
    {
        public List<GameObject> AllGameObjects;
        public static WorldController Instance { get; protected set; }

        public World World { get; protected set; }
        public Sprite floorSprite;
        public GameObject TreeSpritePrefab;

        public List<WorldEntity> CurrentSelection;


        // Use this for initialization
        void Start()
        {
            if (Instance != null)
            {
                Debug.LogError(this.GetType().ToString() + " already instanced");
            }
            Instance = this;
            World = new World(40, 40);
            CurrentSelection = new List<WorldEntity>();

            //generate the other controllers
            InitControllers();

            //generate game object for each tile
            GenerateTiles();

            GenerateTrees();
        }

        private void InitControllers()
        {
            BuildingPrefabController.Instance = new PrefabAssetController<BuildingType>(BuildingPrefabController.GetPaths());
            BuildingIconPrefabController.Instance = new PrefabAssetController<BuildingType>(BuildingIconPrefabController.GetPaths());
            UnitPrefabController.Instance = new PrefabAssetController<UnitType>(UnitPrefabController.GetPaths());
        }

        private void GenerateTiles()
        {
            for (int x = 0; x < World.Width; x++)
            {
                for (int y = 0; y < World.Height; y++)
                {
                    GameObject tile_go = new GameObject();
                    tile_go.name = "Tile_" + x + "_" + y;
                    Tile tile_data = new Tile(new UnityObjectWrapper(tile_go), new Vector2(x, y));
                    World.SetTileAt(x, y, tile_data);

                    //update go position
                    tile_go.transform.position = new Vector3(x, y, 1);
                    tile_go.transform.SetParent(this.transform, true);

                    //add sprite renderer
                    tile_go.AddComponent<SpriteRenderer>();
                    tile_go.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";

                    //register tile update function
                    tile_data.RegisterTileTypeChangedCB((tile) => { OnTileTypeChanges(tile, tile_go); });
                    tile_data.TileType = TileType.DIRT;
                }
            }
        }

        void Update()
        {
            World.Update(Time.deltaTime);
        }

        public void GenerateTrees()
        {
            int numToGen = 80;
            for (int i = 0; i < numToGen; i++)
            {
                var newTree = Instantiate(TreeSpritePrefab);
                newTree.transform.SetParent(this.transform, true);

                new ResourceNode(new UnityObjectWrapper(newTree),
                    new Vector3(UnityEngine.Random.value * World.Width, UnityEngine.Random.value * World.Height),
                    RESOURCE_TYPE.WOOD, 20);
            }
        }

        public void OnTileTypeChanges(Tile inTileData, GameObject inTileGO)
        {
            var sr = inTileGO.GetComponent<SpriteRenderer>();
            //if (inTileData.TileType == TileType.DIRT)
            {
                sr.sprite = floorSprite;
                sr.enabled = true;
            }
        }

    }
}