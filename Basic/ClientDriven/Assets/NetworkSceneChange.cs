using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSceneChange : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NetworkManager.SceneManager.LoadScene("MainMenu",UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
