using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Models.Buildings;

namespace Assets.Controllers.PrefabControllers
{
	public class PrefabAssetController<T>// : MonoBehaviour
	{
        private Dictionary<T, string> Paths;
        private Dictionary<T, GameObject> Prefabs;

        public PrefabAssetController(Dictionary<T, string> inPaths)
        {
            Paths = inPaths;
            Prefabs = new Dictionary<T,GameObject>();

            PreLoadObjects();
        }

        private void PreLoadObjects()
        {
            //for each building type / path, load the gameobject from the path, and store it against the building type
            foreach (var path in Paths)
            {
                LoadResource(path.Key, path.Value);
            }
        }

        private GameObject LoadResource(T inTypeKey, string inPath)
        {
            var obj = Resources.Load(inPath, typeof(GameObject));
            if (obj != null)
            {
                Prefabs.Add(inTypeKey, (GameObject)obj);
                return (GameObject)obj;
            }
            else
            {
                Debug.Log(string.Format("Error; Could not load prefab with type: {0}:{1}, from path '{2}'", typeof(T).ToString(), inTypeKey, inPath));
                return null;
            }
        }

        public GameObject GetPrefab(T inTypeKey)
        {
            //if prefab loaded return
            var obj = Prefabs.ContainsKey(inTypeKey) ? Prefabs[inTypeKey] : null;
            if (obj != null)
            {
                return obj;
            }

            //if path not found error
            if (!Paths.ContainsKey(inTypeKey))
            {
                Debug.LogError(string.Format("No path or prefab found for type: {0}:{1} " + typeof(T).ToString(), inTypeKey.ToString()));
                return null;
            }

            //load resource and return
            obj = LoadResource(inTypeKey, Paths[inTypeKey]);
            return obj;
        }
	}
}
