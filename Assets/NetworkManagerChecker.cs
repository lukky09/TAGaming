using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerChecker : MonoBehaviour
{
    private void Start()
    {
        if (!LobbyManager.instance.IsOnline)
            NetworkManager.Singleton.StartHost();
        else
        {
            if (LobbyManager.instance.IsHosting)
                NetworkManager.Singleton.StartHost();
            else
                NetworkManager.Singleton.StartClient();
        }

    }
}

