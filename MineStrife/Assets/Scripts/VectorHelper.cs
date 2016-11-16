using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class VectorHelper
    {
        public VectorHelper()
        {
            //all class method are static so no real need to create one
        }

        public static Vector2 rotateVector2(Vector2 point, float radians)
        {
            //http://stackoverflow.com/questions/3756920/whats-wrong-with-this-xna-rotatevector2-function
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            return new Vector2(
                point.x * cosRadians - point.y * sinRadians,
                point.x * sinRadians + point.y * cosRadians);
        }

        public static float angleBetween(Vector2 inA, Vector2 inB)
        {
            float dotProd = Vector2.Dot(inA, inB);
            float magA = inA.magnitude;
            float magB = inB.magnitude;

            float raw = dotProd / (magA * magB);
            float thetaRadians = (float)Math.Acos((double)raw);
            return thetaRadians;
        }

        public static float getDistanceBetween(Vector2 inA, Vector2 inB)
        {
            return (inA - inB).magnitude;
        }

        public static Vector2 getHeadingTo(Vector2 inPosition, Vector2 inTarget)
        {
            return inTarget - inPosition;
        }

        public static Vector3 ToVector3(Vector2 inPosition, float inZ)
        {
            return new Vector3(inPosition.x, inPosition.y, inZ);
        }

        public static Vector3 ToVector3(Vector2 inPosition)
        {
            return ToVector3(inPosition, 0);
        }

        public static Vector3 ToVector2(Vector3 inPosition)
        {
            return new Vector2(inPosition.x, inPosition.y);
        }

        public static string ToString(Vector2 inVector, int inPrescision = 2)
        {
            return "(" + Math.Round(inVector.x, inPrescision) + "," + Math.Round(inVector.y, inPrescision) + ")";
        }

    }   
}
