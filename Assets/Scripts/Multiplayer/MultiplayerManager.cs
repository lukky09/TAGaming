using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    private readonly NetworkVariable<FixedString32Bytes> _lobbyMultiplayerName = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> _isLeftGroup = new(writePerm: NetworkVariableWritePermission.Owner);
    private static string _multiplayerName;
    public string MultiplayerName { get { return _multiplayerName; } set { _multiplayerName = value; } }

}
