using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script runs off a timer to spawn the resources in the sim
/// </summary>
public class ResourceSpawn : MonoBehaviour
{
    public float spawnRate = 1;
    public float timeElapsed = 0;
    public float initialSpawn = 5;
    public Vector3 mapSize;

    public GameObject resourceToSpawn;

    private void Start()
    {
        for(int i = 0; i < initialSpawn; i++)
        {
            SpawnResource();
        }
    }

    private void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= spawnRate)
        {
            timeElapsed = timeElapsed % spawnRate;
            SpawnResource();
        }
    }

    void SpawnResource()
    {
        int x = Random.Range(0, (int)mapSize.x);
        int z = Random.Range(0, (int)mapSize.z);
        Instantiate(resourceToSpawn, new Vector3((float)x, .75f, (float)z), Quaternion.identity);
    }
}
