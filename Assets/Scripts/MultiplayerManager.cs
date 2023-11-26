using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    private readonly NetworkVariable<FixedString32Bytes> _lobbyMultiplayerName = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> _isLeftGroup = new(writePerm: NetworkVariableWritePermission.Owner);
    private string _multiplayerName;
    public string MultiplayerName { get { return _multiplayerName; } set { _multiplayerName = value; } }

    private void update()
    {
        if (IsOwner)
        {
            _lobbyMultiplayerName.Value = _multiplayerName;
        }
        else
        {
            _multiplayerName = _lobbyMultiplayerName.Value.ToString();
        }
    }

    public void hostGame()
    {
        Debug.Log("Host dimulai");
        NetworkManager.Singleton.StartHost();
    }

    public void startClient()
    {
        Debug.Log("Client dimulai");
        NetworkManager.Singleton.StartClient();
    }

}
