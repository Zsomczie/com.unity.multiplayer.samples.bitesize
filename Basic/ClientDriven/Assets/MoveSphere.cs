using System;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class MoveSphere : NetworkBehaviour
{
    public event Action ingredientDespawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    public override void OnNetworkDespawn()
    {
        ingredientDespawned?.Invoke();
    }
}
