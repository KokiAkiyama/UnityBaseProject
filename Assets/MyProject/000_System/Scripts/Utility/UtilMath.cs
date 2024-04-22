using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.MathEx
{
    public class MathEx
    {
        //todo: mathex?
        public static bool ContainsLayerInMask(int layer, int mask)
        {
            return ((mask >> layer) & 1) == 1;
        }

        //カーソルからレイを飛ばし、当たったかどうかを返す
        public static bool RayCast(out RaycastHit hitInfo, LayerMask layer)
        {
            return Physics.Raycast(
                Camera.main.ScreenPointToRay(Input.mousePosition),
                out hitInfo,
                float.MaxValue,
                layer
            );
        }

        public static bool Raycast(ref Ray ray, out RaycastHit hitInfo, float maxDistance, LayerMask layer)
        {
            return Physics.Raycast(ray, out hitInfo, maxDistance, layer);
        }

        public static Vector2 Convert2D(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 ConvertAxis2D(Vector3 v)
        {
            return new Vector3(v.x, 0f, v.z);
        }

        public static Vector3 GetDiffAxis2D(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, 0.0f, a.z - b.z);
        }

        public static Vector3 GetDiff(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
    }
};
