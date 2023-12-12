using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerChecker : MonoBehaviour
{
    private void Awake()
    {
		try
		{
			if(LobbyManager.instance.CurrentLobby != null)
			{
				if (LobbyManager.instance.IsHosting)
					NetworkManager.Singleton.StartHost();
				else
                    NetworkManager.Singleton.StartClient();
            }
		}
		catch (System.Exception e)
		{
			Destroy(gameObject);
		}
    }
}

