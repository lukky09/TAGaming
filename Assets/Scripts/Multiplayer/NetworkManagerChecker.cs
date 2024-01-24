using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class NetworkManagerChecker : MonoBehaviour
{
    private void Start()
    {
        //Buat Localhost
        if (!LobbyManager.IsOnline)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(RelayManager.Serverdata);
            if (LobbyManager.instance.IsHosting)
                NetworkManager.Singleton.StartHost();
            else
                NetworkManager.Singleton.StartClient();
        }

    }
}

