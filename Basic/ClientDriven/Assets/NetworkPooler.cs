using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPooler : NetworkBehaviour
{
    [SerializeField] GameObject SpawnedObject;
    [SerializeField] GameObject Spawner;
    [SerializeField] List<GameObject> PooledObjects;
    [SerializeField] float SpawnRate;
    bool spawnerEnabled;
    private void Start()
    {
        PooledObjects = new List<GameObject>();
    }
    private void FixedUpdate()
    {
        if (IsServer && !spawnerEnabled)
        {
            InvokeRepeating(nameof(Spawn), 0f, SpawnRate);
            spawnerEnabled = true;
        }
        if (PooledObjects.Count>10) 
        {
            CancelInvoke(nameof(Spawn));
        }
    }
    void Spawn()
    {
        var instance = Instantiate(SpawnedObject, Spawner.transform.position, Spawner.transform.rotation);
        instance.GetComponent<NetworkObject>().Spawn();
        //instance.SetActive(false);
        PooledObjects.Add(SpawnedObject);
    }
}