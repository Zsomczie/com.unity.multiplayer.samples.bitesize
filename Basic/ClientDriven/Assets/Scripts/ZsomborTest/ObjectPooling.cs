using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectPooling : NetworkBehaviour
{
    [SerializeField] GameObject Spawner;
    [SerializeField] GameObject Prefab;
    [SerializeField] List<GameObject> PooledObjects;
    [SerializeField] float spawnRate;
    bool spawnerEnabled;
    private void Start()
    {
        PooledObjects = new List<GameObject>();
    }
    void FixedUpdate()
    {
        if (IsServer && !spawnerEnabled)
        {
            InvokeRepeating(nameof(SpawnObject), 2f, spawnRate);
            spawnerEnabled = true;

        }
        if (PooledObjects.Count>3&& !PooledObjects[PooledObjects.Count - 1].activeSelf) 
        {
            PooledObjects[PooledObjects.Count-1].SetActive(true);
        }
    }
    void SpawnObject()
    {
        var instance = Instantiate(Prefab, Spawner.transform.position, Spawner.transform.rotation);
        instance.GetComponent<NetworkObject>().Spawn();
        instance.SetActive(false);
        PooledObjects.Add(instance);
    }
}
