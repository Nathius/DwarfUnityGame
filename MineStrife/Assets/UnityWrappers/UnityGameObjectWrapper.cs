using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.UnityWrappers
{
	public class UnityObjectWrapper
	{
        private GameObject ViewObject { get; set; }

        public GameObject GetUnityGameObject()
        {
            return ViewObject;
        }

        public UnityObjectWrapper(GameObject viewObject)
        {
            ViewObject = viewObject;
        }

        public void SetPosition(Vector2 inPosition)
        {
            ViewObject.transform.position = new Vector3(inPosition.x, inPosition.y);
        }

        public void SetColour(Color inColour)
        {
            ViewObject.GetComponent<SpriteRenderer>().color = inColour;
        }
	}
}
