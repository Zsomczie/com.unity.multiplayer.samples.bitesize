using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] GameObject Spawner;
    [SerializeField] GameObject Prefab;
    [SerializeField] float spawnRate;
    bool spawnerEnabled;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsServer&&!spawnerEnabled)
        {
            InvokeRepeating(nameof(SpawnObject), 0f, spawnRate);
            spawnerEnabled = true;
        }
    }
    void SpawnObject() 
    {
        var instance = Instantiate(Prefab, Spawner.transform.position, Spawner.transform.rotation);
        instance.GetComponent<NetworkObject>().Spawn();
    }
}
