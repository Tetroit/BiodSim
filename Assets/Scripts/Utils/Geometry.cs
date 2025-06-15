using System;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TetraUtils
{
    [System.Serializable]
    public struct OBB
    {
        public Vector2 pos;
        public Vector2 a1;
        public Vector2 a2;


        public readonly Vector2 p0 => pos;
        public readonly Vector2 p1 => pos + a1;
        public readonly Vector2 p2 => pos + a2;
        public readonly Vector2 p3 => pos + a1 + a2;

        /// <summary>
        /// corners coords in local space
        /// </summary>
        public readonly Vector2[] corners => new[] { p0, p1, p2, p3 };
        /// <summary>
        /// corner coords in globas space 
        /// </summary>
        //public readonly Vector2[] cornersGS => new[] { p0 + pos, p1 + pos, p2 + pos, p3 + pos };

        public readonly Vector2 center => pos + ((a1 + a2)/2);

        public OBB(Vector2 a1, Vector2 a2, Vector2 pos)
        {
            this.a1 = a1;
            this.a2 = a2;
            this.pos = pos;
        }
        public OBB(Rect rect)
        {
            a1 = new Vector2(rect.width, 0);
            a2 = new Vector2(0, rect.height);
            pos = rect.position;
        }
        public OBB(Rect rect, float angle)
        {
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);
            a1 = rect.width * new Vector2(-cos, sin);
            a2 = rect.height * new Vector2(sin, cos);
            pos = rect.position;
        }

        public static OBB RandomOBB(Rect bounds, float maxSize, float minSize)
        {
            Vector2 size = new Vector2(
                Random.Range(minSize, maxSize),
                Random.Range(minSize, maxSize));

            Vector2 pos = new Vector2(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax));

            float angle = Random.Range(0, 6.283f);
            Rect newArea = new Rect(pos, size);

            return new OBB(newArea, angle);
        }
        public static implicit operator OBB(Rect rect) => new OBB(rect);

        public bool Contains(Vector2 point)
        {
            Vector2 d = point - pos;
            float sample = d.ProjectNormalized(a1);
            if (sample > 1 || sample < 0) return false;
            sample = d.ProjectNormalized(a2);
            if (sample > 1 || sample < 0) return false;
            return true;
        }
        public Vector2 InLocal(Vector2 point)
        {
            Vector2 d = point - pos;
            return new Vector2(d.ProjectNormalized(a1), d.ProjectNormalized(a2));
        }
        public Vector2 InLocalNormalized(Vector2 point)
        {
            Vector2 d = point - pos;
            return new Vector2(d.ProjectNormalized(a1.normalized), d.ProjectNormalized(a2.normalized));
        }
        public float ToClosestPoint(Vector2 point)
        {
            Vector2 dist = point - center;
            float a1l = a1.magnitude;
            float a2l = a2.magnitude;
            Vector2 a1n = a1 / a1l;
            Vector2 a2n = a2 / a2l;
            Vector2 local = new Vector2(dist.ProjectNormalized(a1n), dist.ProjectNormalized(a2n));
            Vector2 d = local.Abs() - new Vector2(a1l*0.5f, a2l*0.5f);
            return (GeometryUtils.Max(d, Vector2.zero).magnitude + Mathf.Min(Mathf.Max(d.x, d.y),0.0f));
        }
        public void Draw()
        {
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p0, p2);
            Gizmos.DrawLine(p3, p1);
            Gizmos.DrawLine(p3, p2);
        }
        public void DrawAsCube(float height, float zoffset = 0)
        {
            Vector3[] v3d = new[]{
                new Vector3(p0.x, zoffset, p0.y),
                new Vector3(p1.x, zoffset, p1.y),
                new Vector3(p2.x, zoffset, p2.y),
                new Vector3(p3.x, zoffset, p3.y),
                new Vector3(p0.x, zoffset + height, p0.y),
                new Vector3(p1.x, zoffset + height, p1.y),
                new Vector3(p2.x, zoffset + height, p2.y),
                new Vector3(p3.x, zoffset + height, p3.y),
            };
            Gizmos.DrawLine(v3d[0], v3d[1]);
            Gizmos.DrawLine(v3d[0], v3d[2]);
            Gizmos.DrawLine(v3d[3], v3d[1]);
            Gizmos.DrawLine(v3d[3], v3d[2]);

            Gizmos.DrawLine(v3d[0], v3d[4]);
            Gizmos.DrawLine(v3d[1], v3d[5]);
            Gizmos.DrawLine(v3d[2], v3d[6]);
            Gizmos.DrawLine(v3d[3], v3d[7]);

            Gizmos.DrawLine(v3d[4], v3d[5]);
            Gizmos.DrawLine(v3d[4], v3d[6]);
            Gizmos.DrawLine(v3d[7], v3d[5]);
            Gizmos.DrawLine(v3d[7], v3d[6]);
        }
    }


    public static class GeometryUtils
    {
        public static T GetLong<T>(int orientation, T width, T height)
        {
            if (orientation % 2 == 0) return width;
            return height;
        }
        public static T GetShort<T>(int orientation, T width, T height)
        {
            if (orientation % 2 == 0) return height;
            return width;
        }
        public static Vector2Int Random2(Vector2Int minIncl, Vector2Int maxIncl)
        {
            return new Vector2Int(Random.Range(minIncl.x, maxIncl.x+1), Random.Range(minIncl.y, maxIncl.y+1));
        }
        public static Vector2 Random2(Vector2 minIncl, Vector2 maxExcl)
        {
            return new Vector2(Random.Range(minIncl.x, maxExcl.x), Random.Range(minIncl.y, maxExcl.y));
        }
        public static Vector3Int Random3(Vector3Int minIncl, Vector3Int maxIncl)
        {
            return new Vector3Int(Random.Range(minIncl.x, maxIncl.x + 1), Random.Range(minIncl.y, maxIncl.y + 1), Random.Range(minIncl.z, maxIncl.z + 1));
        }
        public static Vector3 Random3(Vector3 minIncl, Vector3 maxExcl)
        {
            return new Vector3(Random.Range(minIncl.x, maxExcl.x), Random.Range(minIncl.y, maxExcl.y), Random.Range(minIncl.z, maxExcl.z));
        }


        public static float InvCDFSin(float fac01)
        {
            return Mathf.Acos(1 - 2 * fac01);
        }
        /// <summary>
        /// gives a random vertor on a unit sphere
        /// </summary>
        /// <returns></returns>
        public static Vector3 Random3US()
        {
            float theta = Random.Range(0f, Mathf.PI * 2f);
            float phi = InvCDFSin(Random.Range(0f, 1f));
            return new Vector3(Mathf.Sin(theta) * MathF.Sin(phi), Mathf.Cos(phi), Mathf.Cos(theta) * MathF.Sin(phi));
        }
        public static Vector3 MapToWorld(Vector2 p, float h = 0)
        {
            return new Vector3(p.x, h, p.y);
        }
        public static Vector2 WorldToMap(Vector3 p)
        {
            return new Vector2(p.x, p.z);
        }
        public static Vector3 MapToTerrain(Vector2 p, Terrain t)
        {
            Vector3 res = new Vector3(p.x, 0, p.y);
            res.y = t.SampleHeight(res);
            return res;
        }
        public static float Cross(this Vector2 a1, Vector2 a2)
        {
            return a1.x * a2.y - a1.y * a2.x;
        }
        public static Vector2 Abs(this Vector2 a)
        {
            return new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
        }
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
        }
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }
        public static Vector2 Project(this Vector2 a1, Vector2 a2)
        {
            return a2 * ProjectNormalized(a1, a2);
        }
        public static float ProjectNormalized(this Vector2 a1, Vector2 a2)
        {
            return (Vector2.Dot(a1, a2) / Vector2.SqrMagnitude(a2));
        }
        public static bool OBBOverlaping(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, Vector2 aPos, Vector2 bPos)
        {
            Vector2[] aCorners = new Vector2[] { aPos, aPos + a1, aPos + a2, aPos + a1 + a2 };
            Vector2[] bCorners = new Vector2[] { bPos, bPos + b1, bPos + b2, bPos + b1 + b2 };

            return OBBOverlaping(aCorners, bCorners, a1, a2);
        }

        public static bool OBBOverlaping(OBB obb1, OBB obb2)
        {
            Vector2[] aCorners = obb1.corners;
            Vector2[] bCorners = obb2.corners;

            return OBBOverlaping(aCorners, bCorners, obb1.a1, obb1.a2);
        }
        /// <summary>
        /// segments 0 - 3, 1 - 2 are diagonals
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool OBBOverlaping(Vector2[] a, Vector2[] b)
        {
            return OBBOverlaping(a, b, a[1] - a[0], a[2] - a[0]);
        }
        static bool OBBOverlaping(Vector2[] a, Vector2[] b, Vector2 a1, Vector2 a2)
        {
            Vector2 projLine = a1;
            float sample;
            float min = float.MaxValue;
            float max = float.MinValue;
            for (int i = 0; i < b.Length; i++)
            {
                sample = ProjectNormalized(b[i] - a[0], projLine);
                if (sample < min) min = sample;
                if (sample > max) max = sample;
            }
            if (min > 1 || max < 0) return false;

            projLine = a2;
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < b.Length; i++)
            {
                sample = ProjectNormalized(b[i] - a[0], projLine);
                if (sample < min) min = sample;
                if (sample > max) max = sample;
            }
            if (min > 1 || max < 0) return false;

            return true;
        }

        public static bool BlockInRange(this RectInt rect, Vector2Int vec)
        {
            if (vec.x >= rect.xMax) return false;
            if (vec.x < rect.xMin) return false;
            if (vec.y >= rect.yMax) return false;
            if (vec.y < rect.yMin) return false;
            return true;
        }
        public static bool PointInRange(this RectInt rect, Vector2Int vec)
        {
            if (vec.x > rect.xMax) return false;
            if (vec.x < rect.xMin) return false;
            if (vec.y > rect.yMax) return false;
            if (vec.y < rect.yMin) return false;
            return true;
        }
    }
}
