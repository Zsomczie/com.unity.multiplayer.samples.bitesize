using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    [SerializeField] GameObject SpawnedObject;
    [SerializeField] GameObject Spawner;
    [SerializeField] float SpawnRate;
    bool spawnerEnabled;
    private void FixedUpdate()
    {
        if (IsServer && !spawnerEnabled) 
        {
            InvokeRepeating(nameof(Spawn), 0f, SpawnRate);
            spawnerEnabled = true;
        }
    }
    void Spawn() 
    {
       var instance = Instantiate(SpawnedObject, Spawner.transform.position,Spawner.transform.rotation);
        instance.GetComponent<NetworkObject>().Spawn();
    }
}
