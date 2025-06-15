using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public const int memorySize = 6 * sizeof(float);
    [StructLayout(LayoutKind.Sequential)]
    [System.Serializable]
    public struct Data
    {
        public Vector3 pos;
        public Vector3 dir;
    }
    public Vector3 dir;
    public float speed = 1.0f;

    // Update is called once per frame
    public void UpdateSim(BoidManager manager)
    {
        Repel(manager);
        Flock(manager);

        transform.position += Time.deltaTime * speed * dir;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Repel(BoidManager manager)
    {
        var neighbours = manager.GetClosestDist(transform.position, manager.repelDist, this);
        foreach (var neighbour in neighbours)
        {
            Vector3 dist = neighbour;
            float r2 = dist.sqrMagnitude;
            if (r2 == 0) r2 = 0.001f;
            Vector3 repelDir = dist / Mathf.Sqrt(r2);
            dir -= (Time.deltaTime * manager.repellingFac / r2) * repelDir;
            dir.Normalize();
        }
        
    }
    void Flock(BoidManager manager)
    {
        var newDir = manager.GetAverage(transform.position, manager.flockDist, this);
        dir = Vector3.Slerp(dir, newDir.Item1 + (manager.clutchingFac * newDir.Item2.normalized) , Time.deltaTime * manager.flockingFac).normalized;
    }

    [ExecuteAlways]
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Vector3.zero, Vector3.forward);
    }

    public Data GenerateData()
    {
        var data = new Data();
        data.pos = transform.position;
        data.dir = dir;
        return data;
    }
    public void ApplyData(Data data)
    {
        dir = data.dir;
        transform.position = data.pos;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
