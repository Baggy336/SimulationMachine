using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles spawning the scavenger into the sim. Should be refactored to spawn any kind of creature
/// </summary>
public class ScavengerSpawn : MonoBehaviour
{
    public GameObject scavenger;
    public GameObject[] scavengerList;

    public Vector3 mapSize;

    void FixedUpdate()
    {
        scavengerList = GameObject.FindGameObjectsWithTag("scavenger");

        if(scavengerList.Length < 1)
        {
            SpawnScavenger();
        }
    }

    void SpawnScavenger()
    {
        int x = Random.Range(0, (int)mapSize.x);
        int z = Random.Range(0, (int)mapSize.z);
        Instantiate(scavenger, new Vector3((float)x, .75f, (float)z), Quaternion.identity);
    }
}
