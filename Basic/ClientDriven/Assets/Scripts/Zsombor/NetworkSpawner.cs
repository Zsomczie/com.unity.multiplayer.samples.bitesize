using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    [SerializeField] Transform spawner;
    [SerializeField] GameObject spawnedPrefab;
    [SerializeField] List<GameObject> pooledObjects;
    [SerializeField] float spawnDelay;
    bool spawnerEnabled;
    int counter;
    private void FixedUpdate()
    {
        if (!spawnerEnabled&&IsServer&&counter<8) 
        {
            InvokeRepeating(nameof(SpawnObject), 2, spawnDelay);
            spawnerEnabled = true;
        }
    }
    void SpawnObject() 
    {
       var instance=Instantiate(spawnedPrefab, spawner.position, spawner.rotation);
        instance.GetComponent<NetworkObject>().Spawn();
        //instance.SetActive(false);
        pooledObjects.Add(instance);
        counter++;
    }
}
