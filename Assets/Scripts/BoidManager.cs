using System.Collections;
using System.Collections.Generic;
using TetraUtils;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class BoidManager : MonoBehaviour
{
    const int MAX_AMOUNT = 1024;
    [Range(0, MAX_AMOUNT)]
    public int spawnAmount = 100;
    public List<Boid> boids = new();
    public GameObject prefab;
    BoxCollider spawnZone;

    public float repellingFac = 1;
    public float flockingFac = 1;
    public float clutchingFac = 1;
    public float repelDist = 0.5f;
    public float flockDist = 1.0f;

    public float speed = 1f;

    public enum ComputeMode
    {
        CPU = 0,
        GPU = 1,
    }
    public ComputeMode computeMode = ComputeMode.CPU;

    public ComputeShader shader;
    const int THREADSX = 10;
    const int THREADSY = 1;

    private void Awake()
    {
        spawnZone = GetComponent<BoxCollider>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Spawn(spawnAmount);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Spawn();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Delete(spawnAmount);
        }
        if (computeMode == ComputeMode.CPU)
        {
            foreach (var boid in boids)
            {
                boid.UpdateSim(this);
            }
        }
        if (computeMode == ComputeMode.GPU)
        {
            ComputeGPU();
        }
        foreach (var boid in boids)
        {
            ResolveBorders(boid);
        }
    }
    public void ComputeGPU()
    {
        if (boids.Count <= 0) return;
        Boid.Data[] array = new Boid.Data[boids.Count];
        for (int i = 0; i < boids.Count; i++)
        {
            array[i] = boids[i].GenerateData();
        }
        shader.SetFloat("repellingFac", repellingFac);
        shader.SetFloat("flockingFac", flockingFac);
        shader.SetFloat("clutchingFac", clutchingFac);
        shader.SetFloat("repelDist", repelDist);
        shader.SetFloat("flockDist", flockDist);
        shader.SetInt("elementCount", boids.Count);
        shader.SetFloat("_DeltaTime", Time.deltaTime);

        shader.SetVector("min", spawnZone.bounds.min);
        shader.SetVector("max", spawnZone.bounds.max);

        ComputeBuffer buffer = new ComputeBuffer(array.Length, Boid.memorySize);
        buffer.SetData(array);

        shader.SetBuffer(0, "boids", buffer);
        shader.Dispatch(0, 1, 1, 1);

        buffer.GetData(array);
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].ApplyData(array[i]);
        }
        buffer.Dispose();
    }
    public void ResolveBorders(Boid boid)
    {
        Bounds bounds = spawnZone.bounds;
        Vector3 boidPos = boid.transform.position;
        if (bounds.Contains(boidPos)) return;
        Vector3 boundsDir = bounds.ClosestPoint(boidPos) - boidPos;
        boid.dir = Vector3.Slerp(boid.dir, boundsDir, Time.deltaTime * 3);
    }
    public void Spawn(int amount)
    {
        if (amount > MAX_AMOUNT) amount = MAX_AMOUNT;
        if (amount == 0) return;

        for (int i = 0; i < amount; i++)
            Spawn();
    }
    void Spawn()
    {
        Bounds bounds = spawnZone.bounds;
        Vector3 pos = GeometryUtils.Random3(bounds.min, bounds.max);
        Boid boid = Instantiate(prefab, pos, Quaternion.identity, transform).GetComponent<Boid>();
        boid.name = prefab.name + " " + boids.Count.ToString("00000");
        boid.dir = GeometryUtils.Random3US();
        boid.speed = speed;
        boids.Add(boid);
    }

    void Delete(int amount)
    {
        if (amount > boids.Count) amount = boids.Count;
        if (amount == 0) return;

        for (int i=0; i < amount; i++)
        {
            Boid boid = boids[i];
            Destroy(boid.gameObject);
        }
        boids.RemoveRange(0, amount);
    }

    public List<Vector3> GetClosestDist(Vector3 pos, float radius = 1, Boid self = null)
    {
        List<Vector3> list = new();
        foreach(Boid boid in boids)
        {
            if (boid == self) continue;
            Vector3 dir = boid.transform.position - pos;
            float dist = dir.magnitude;
            if (dist < radius)
                list.Add(dir);
        }
        return list;
    }
    public (Vector3,Vector3) GetAverage(Vector3 pos, float radius = 1, Boid self = null)
    {
        Vector3 avDir = Vector3.zero;
        Vector3 avPos = Vector3.zero;

        int count = 0;
        foreach (Boid boid in boids)
        {
            if (boid == self) continue;
            Vector3 dir = boid.transform.position - pos;
            float dist = dir.magnitude;
            if (dist < radius)
            {
                count++;
                avDir += boid.dir / dist;
                avPos += boid.transform.position;
            }
        }
        return (avDir.normalized, avPos/count - pos);
    }
}
