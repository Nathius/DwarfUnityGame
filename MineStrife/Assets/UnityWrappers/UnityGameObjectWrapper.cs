using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts;

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
            var viewPosition = VectorHelper.ToVector3(GridHelper.GridToIsometric(inPosition));
            ViewObject.transform.position = viewPosition;
            SetOrderInLayer(-(int)(inPosition.y * 100));
        }

        private void SetOrderInLayer(int inZSort)
        {
            var renderer = ViewObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = inZSort;
            }
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

        public Vector2? GetSpriteSize()
        {
            var renderer = ViewObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                return new Vector2(renderer.bounds.extents.x * 2, renderer.bounds.extents.y * 2);
            }
            return null;
        }

        public Vector2? GetSpriteCenter()
        {
            var renderer = ViewObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                return new Vector2(renderer.bounds.center.x, renderer.bounds.center.y);
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
                box.offset = new Vector2(0, (inSize.y * 0.5f));
            }
        }

        public void SetColliderState(bool isActive)
        {
            var collider = ViewObject.GetComponent<BoxCollider2D>();
            if(collider != null)
            {
                collider.enabled = isActive;
            }
        }
	}
}
