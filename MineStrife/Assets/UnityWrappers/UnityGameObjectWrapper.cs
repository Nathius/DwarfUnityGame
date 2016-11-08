﻿using System;
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

        public Vector2? GetBoundSize()
        {
            var box = ViewObject.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                return box.size;
            }
            return null;
        }

        public void SetDisplaySize(float inWidth, float inHeight)
        {
            //get sprite size
            //find scale to achieve size
            //update bounds if relavent
            //update scale
        }

        public void SetBoundSize(Vector2 inSize)
        {
            var box = ViewObject.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                box.size = inSize;
                box.offset = inSize * 0.5f;
            }
        }
	}
}
