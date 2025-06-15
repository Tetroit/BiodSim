using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace TetraUtils
{
	[System.Serializable]
    public class Path
    {
        public Color color;
        public List<Vector3> points = new();
		public int size => points.Count;
        public Path()
        {
            color = Color.white;
        }
		public void AddPoint(Vector3 p)
		{
			points.Add(p);
        }
		public void InsertPoint(int index, Vector3 p)
		{
            points.Insert(index, p);
        }
		public void Subdivide(int index)
		{
			points.Insert(index+1, (points[index] + points[index + 1]) / 2);
        }
		public void RemovePoint(int index)
		{
            points.RemoveAt(index);
        }
        public Vector3 GetPoint(int pointIndex)
		{
			if (pointIndex < 0 || pointIndex >= points.Count)
			{
				Debug.Log("Curve.cs: WARNING: pointIndex out of range: " + pointIndex + " curve length: " + points.Count);
				return Vector3.zero;
			}
			return points[pointIndex];
		}

		public void Draw()
		{
			Gizmos.color = color;
            for (int i = 0; i < points.Count - 1; i++)
            {
				Gizmos.DrawSphere(points[i], 0.5f);
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
			Gizmos.DrawSphere(points[points.Count - 1], 0.5f);
        }
		public void Draw(Matrix4x4 transform)
		{
			Gizmos.matrix = transform;
			Draw();
		}

		public struct CubicBezier
		{
			public List<Vector3> points;

			public Vector3 Sample(float fac)
			{
				Vector3 res = Vector3.zero;

				return res;
			}
		}
	}
}

